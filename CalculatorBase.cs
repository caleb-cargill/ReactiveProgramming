using NLua;
using ReactiveProgramming.Models;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ReactiveProgramming;

public class CalculatorBase
{
    private List<ICalculationObject> subjects;

    public CalculatorBase(List<ICalculationObject> subjects)
    {
        this.subjects = subjects;
    }

    public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var calcs = DbContext.GetCalculations();
        var propertyName = e.PropertyName;
        var toRun = calcs.Where(c => c.MemberArguments.Contains(propertyName)).ToList();
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

    public virtual string? Calculate(object subject, Calculation calculation)
    {
        using (Lua state = new Lua())
        {
            foreach (var arg in calculation.MemberArguments)
                state[arg] = GetArgValue(arg, subject);

            string body = calculation.Body;

            state.DoString(body);

            string? result = state[$"{calculation.Updates}"]?.ToString();
            return result;
        }
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