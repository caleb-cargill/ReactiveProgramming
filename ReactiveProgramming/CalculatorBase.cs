using NLua;
using ReactiveProgramming.Models;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace ReactiveProgramming;

public class CalculatorBase
{
    private List<ICalculationObject> subjects;

    private List<ObjectNode> nodes;

    public CalculatorBase(List<ICalculationObject> subjects)
    {
        this.subjects = subjects;
    }

    public CalculatorBase(List<ObjectNode> nodes)
    {
        this.nodes = nodes;
        this.subjects = nodes.Select(n => n.TargetCalculationObject).ToList();
    }

    public void HandleObjectNodeChanged(ObjectNode node, string propName)
    {
        var calcs = DbContext.GetCalculations();
        var propertyName = propName;
        var senderName = (node.TargetCalculationObject?.GetType() ?? node.Children?.GetType())?.Name;
        var toRun = calcs.Where(c => c.MemberArguments.Any(m => m.Subject == senderName && m.Member == propertyName)).ToList();
        foreach (var calc in toRun)
        {
            var target =
                nodes.Where(n => (n.Equals(node) || n.GetAllChildren().Any(c => c.Equals(node))) && n.TargetCalculationObject?.GetType().GetProperty(calc.Updates) != null && n.TargetCalculationObject?.GetType().Name == calc.UpdatesSubject).FirstOrDefault();
            if (target == null) return;
            var result = Calculate(node, target, calc)?.ToString();
            Console.WriteLine($"Calculation on {target}: {calc.Name} = {result}. Triggered by {propertyName}.");
            var prop = target.TargetCalculationObject.GetType().GetProperty(calc.Updates);
            prop?.SetValue(target.TargetCalculationObject, GetTypedValue(prop, result));
        }
    }

    public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var calcs = DbContext.GetCalculations();
        var propertyName = e.PropertyName;
        var senderName = sender.GetType().Name;
        var toRun = calcs.Where(c => c.MemberArguments.Any(m => m.Subject == senderName && m.Member == propertyName)).ToList();
        foreach (var calc in toRun)
        {
            var targetSubjects =
                subjects
                .Where(o =>
                    o.GetType().GetProperty(calc.Updates) != null);
            if (!targetSubjects.Any())
                targetSubjects =
                subjects
                .Where(o =>
                    o.GetType().GetProperty(calc.Updates) != null
                    && o.GetType().GetProperties().Any(p => Nullable.Equals(p.GetValue(o), sender)));
            if (!targetSubjects.Any()) continue;
            foreach (var target in targetSubjects)
            {
                var result = Calculate(target, calc).ToString();
                Console.WriteLine($"Calculation: {calc.Name} = {result}. Triggered by {propertyName}.");
                var prop = target.GetType().GetProperty(calc.Updates);
                prop?.SetValue(target, GetTypedValue(prop, result));
            }
        }
    }

    public virtual string? Calculate(ObjectNode sender, ObjectNode target, Calculation calculation)
    {
        using (Lua state = new Lua())
        {
            foreach (var arg in calculation.MemberArguments)
                state[arg.Member] = GetArgValue(arg.Member, target, sender);

            string body = calculation.Body;

            state.DoString(body);

            string? result = state[$"{calculation.Updates}"]?.ToString();
            return result;
        }
    }

    public virtual string? Calculate(object subject, Calculation calculation)
    {
        using (Lua state = new Lua())
        {
            foreach (var arg in calculation.MemberArguments)
                state[arg.Member] = GetArgValue(arg.Member, subject);

            string body = calculation.Body;

            state.DoString(body);

            string? result = state[$"{calculation.Updates}"]?.ToString();
            return result;
        }
    }

    private object? GetArgValue(string arg, ObjectNode node, ObjectNode sender)
    {
        var targetProperty = node.GetAllChildren().Where(c => c.TargetCalculationObject?.GetType().GetProperty(arg) != null && c.Equals(sender)).FirstOrDefault();

        if (targetProperty != null)
            return targetProperty.TargetCalculationObject?.GetType()?.GetProperty(arg)?.GetValue(targetProperty.TargetCalculationObject);

        throw new InvalidOperationException(message: $"Value for {arg} on {node} not found.");
    }

    private object? GetArgValue(string arg, object subject)
    {
        var prop = subject.GetType().GetProperty(arg);
        if (prop != null)
            return prop?.GetValue(subject);
        else
        {
            var child = subject.GetType().GetProperties().Where(p => p.PropertyType.GetProperty(arg) != null).FirstOrDefault()?.GetValue(subject);
            return child.GetType().GetProperty(arg)?.GetValue(child);
        }
    }

    private object? GetTypedValue(PropertyInfo prop, string value)
        => prop.PropertyType.Name switch
        {
            "Int32" => int.Parse(value),
            "Double" => double.Parse(value),
            "String" => value,
            _ => null
        };
}