using System.ComponentModel;
using System.Reactive;

namespace ReactiveProgramming;

public class CalculationObserver : IObserver<EventPattern<PropertyChangedEventArgs>>
{
    private CalculatorBase calculator;

    public CalculationObserver(List<ICalculationObject> objects)
    {
        calculator = new CalculatorBase(objects);
    }

    public void OnCompleted()
        => Console.WriteLine("Calculations Completed");

    public void OnError(Exception error)
        => Console.WriteLine($"Error occurred: {error.Message}");

    public void OnNext(EventPattern<PropertyChangedEventArgs> e)
        => calculator.HandlePropertyChanged(e.Sender, e.EventArgs);
}