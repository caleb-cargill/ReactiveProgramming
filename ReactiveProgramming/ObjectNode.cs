using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveProgramming;

public class ObjectNode
{

    public ICalculationObject? TargetCalculationObject { get; private set; }

    public ObjectNode(ICalculationObject targetObject)
    {
        Initialize(targetObject);
    }

    public ObjectNode(List<ICalculationObject> targetObjects)
    {
        Initialize(targetObjects);
    }

    public Guid UniqueId { get; private set; }

    public List<ObjectNode>? Children { get; private set; }

    public IObservable<ObjectNodeChangedArgs>? ChangesObservable { get; private set; }

    public List<ObjectNode> GetAllChildren()
    {
        var nodes = new List<ObjectNode>() { this };
        nodes.AddRange(Children?.SelectMany(c => c.GetAllChildren()) ?? new List<ObjectNode>());
        return nodes;
    }

    private void Initialize(object targetObject)
    {
        if (targetObject is ICalculationObject co)
            this.TargetCalculationObject = co;
        else if (targetObject is IList li)
            this.Children = li.Cast<ICalculationObject>().Select(co => new ObjectNode(co)).ToList();
        UniqueId = Guid.NewGuid();
        Children ??= CreateChildrenNodes();
        ChangesObservable =
            this.TargetCalculationObject != null
            ? Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => this.TargetCalculationObject.PropertyChanged += h,
                    h => this.TargetCalculationObject.PropertyChanged -= h)
                .Select(e => new ObjectNodeChangedArgs() { Node = this, Sender = e.Sender as ICalculationObject, PropertyName = e?.EventArgs.PropertyName ?? string.Empty })
            : Children.ToObservable().SelectMany(c =>
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => c.TargetCalculationObject.PropertyChanged += h,
                    h => c.TargetCalculationObject.PropertyChanged -= h))
            .Select(e => new ObjectNodeChangedArgs() { Node = this, Sender = e.Sender as ICalculationObject, PropertyName = e?.EventArgs.PropertyName ?? string.Empty });
        var observer = new ObjectObserver(GetAllChildren());
        ChangesObservable.Subscribe(observer);
    }

    private List<ObjectNode> CreateChildrenNodes()
        => TargetCalculationObject?
            .GetType()
            .GetProperties()
            .Where(p => IsCalculationObjectType(p.PropertyType))
            .Select(p => GetObjectNode(p.GetValue(TargetCalculationObject)))
            .ToList()
            ?? new List<ObjectNode>();

    private bool IsCalculationObjectType(Type type)
        => type.GetInterfaces().Contains(typeof(ICalculationObject)) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));

    private ObjectNode GetObjectNode(object? value)
    {
        if (value is ICalculationObject co)
            return new ObjectNode(co);
        else if (value is IList li)
            return new ObjectNode(li.Cast<ICalculationObject>().ToList());
        else
            throw new InvalidOperationException(message: $"{value?.GetType().FullName} is not supported for type {typeof(ObjectNode).FullName}");
    }

    public override string ToString()
    {
        return $"{UniqueId} - Type: {TargetCalculationObject?.GetType().FullName ?? Children.GetType().FullName}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is ObjectNode node)
            return node.GetHashCode() == this.GetHashCode();
        else
            return false;
    }

    public override int GetHashCode()
        => UniqueId.GetHashCode();
}

public class ObjectNodeChangedArgs
{
    public ICalculationObject? Sender { get; set; }
    public string? PropertyName { get; set; }
    public ObjectNode? Node { get; set; }
}

