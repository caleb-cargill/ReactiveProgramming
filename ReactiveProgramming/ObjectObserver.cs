using System;
using System.ComponentModel;
using System.Reactive;

namespace ReactiveProgramming;

public class ObjectObserver : IObserver<ObjectNodeChangedArgs>
{
    private CalculatorBase calculator;

    public ObjectObserver(List<ObjectNode> nodes)
    {
        calculator = new CalculatorBase(nodes);
    }

    public void OnCompleted()
    => Console.WriteLine("Calculations Completed");

    public void OnError(Exception error)
        => Console.WriteLine($"Error occurred: {error.Message}");

    public void OnNext(ObjectNodeChangedArgs e)
        => calculator.HandleObjectNodeChanged(e.Node, e.PropertyName);
}

