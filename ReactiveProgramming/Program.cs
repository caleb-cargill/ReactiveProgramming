using System.ComponentModel;
using System.Reactive.Linq;

namespace ReactiveProgramming;

partial class Program
{
    static void Main(string[] args)
    {
        Test_ObjectNodes_Observable_ManySubjects();

        Console.ReadLine();
    }

    private static void Test_SimpleCollectionObservable()
    {

        // Arrange
        var subjects = DbContext.GetSubjects();
        var members = subjects.SelectMany(s => s.Members).ToList();
        var calculations = DbContext.GetCalculations();

        // Act
        var subjectStream = subjects.ToObservable();
        subjectStream.Subscribe(s => Console.WriteLine(s.Name));

        var memberStream = subjectStream.SelectMany(s => s.Members);
        memberStream.Subscribe(m => Console.WriteLine(m.Name));

        var calculationStream = memberStream.Select(m => calculations.FirstOrDefault(c => c.Updates == m.Name));
        calculationStream.Subscribe(c => Console.WriteLine(c?.Name));
    }

    private static void Test_SimplePropertyChangedObservable()
    {
        // Arrange
        var cube = Source.GetCube();

        // Act
        var propertyChangedEvents =
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => cube.PropertyChanged += h,
                h => cube.PropertyChanged -= h);
        var propertyChangedObserver =
            propertyChangedEvents.Subscribe(e => Console.WriteLine("Property Changed: " + e.EventArgs.PropertyName));

        cube.Height = 5;
        cube.Volume = 125;
        cube.Height = 10;
        cube.Volume = 1000;

        propertyChangedObserver.Dispose();

        cube.Height = 20;
        cube.Volume = 8000;
    }

    private static void Test_SimplePropertyChangedObservable_CalcSubscriber()
    {
        // Arrange
        var calcs = DbContext.GetCalculations();
        var objects = new List<ICalculationObject>() { Source.GetSquare() };
        var calculator = new CalculatorBase(objects);
        var square = objects.OfType<Square>().First();

        // Act
        var propertyChangedEvents =
           objects.ToObservable()
                .SelectMany(o =>
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => o.PropertyChanged += h,
                        h => o.PropertyChanged -= h));

        var propertyChangedObserver =
            propertyChangedEvents.Subscribe(e => calculator.HandlePropertyChanged(e.Sender, e.EventArgs));

        square.Side = 10;
    }

    private static void Test_SimplePropertyChangedObservable_CalcSubscriber_TwoSubjects()
    {
        // Arrange
        var square = Source.GetSquare();
        var objects = new List<ICalculationObject>() { square, new Cube() { Face = square } };

        // Act
        var propertyChangedEvents =
            objects.ToObservable()
                .SelectMany(o =>
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => o.PropertyChanged += h,
                        h => o.PropertyChanged -= h));

        var propertyChangedObserver =
            propertyChangedEvents.Subscribe(new CalculationObserver(objects));
        
        square.Side = 10;
    }

    private static void Test_SimplePropertyChangedObservable_CalcSubscriber_ManySubjects()
    {
        // Arrange
        var square = Source.GetSquare();
        var cube1 = new Cube() { Face = square };
        var cube2 = new Cube() { Face = square };
        var cube3 = new Cube() { Face = square };
        var cube4 = new Cube() { Face = square };
        var objects = new List<ICalculationObject>() { square, cube1, cube2, cube3, cube4, new RubixCube() { Cubes = new List<Cube>() { cube1, cube2, cube3, cube4 } } };

        // is this where a subject tree needs to come into play?
        // is this where a calculation tree needs to come into play?    
        // RubixCube
        //      Cube1
        //          Face
        //      Cube2
        //          Face
        //      Cube3
        //          Face
        //      Cube4
        //          Face        

        // Act
        var propertyChangedEvents =
            objects.ToObservable()
                .SelectMany(o =>
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => o.PropertyChanged += h,
                        h => o.PropertyChanged -= h));

        var propertyChangedObserver =
            propertyChangedEvents.Subscribe(new CalculationObserver(objects));

        square.Side = 10;
    }

    private static void Test_ObjectNodes_ManySubjects()
    {
        // Arrange
        var square = Source.GetSquare();
        var cube1 = new Cube() { Face = square };
        var cube2 = new Cube() { Face = square };
        var cube3 = new Cube() { Face = square };
        var cube4 = new Cube() { Face = square };
        var rb = new RubixCube() { Cubes = new List<Cube>() { cube1, cube2, cube3, cube4 } };

        // Act
        var on = new ObjectNode(rb);
        foreach (var node in on.GetAllChildren())
            Console.WriteLine(node.ToString());

    }

    private static void Test_ObjectNodes_Observable_ManySubjects()
    {
        // Arrange
        var square = Source.GetSquare();
        var cube1 = new Cube() { Face = square };
        var cube2 = new Cube() { Face = square };
        var cube3 = new Cube() { Face = square };
        var cube4 = new Cube() { Face = square };
        var rb = new RubixCube() { Cubes = new List<Cube>() { cube1, cube2, cube3, cube4 } };

        // Act
        var on = new ObjectNode(rb);
        var children = on.GetAllChildren();
        var childObservables = on.GetAllChildren().Select(c => c.ChangesObservable).Where(c => c != null);
        var observable = Observable.Merge(childObservables);
        var observer = observable.Subscribe(p => Console.WriteLine($"PropertyChanged: {p.PropertyName}"));
        var objectobserver = new ObjectObserver(on.GetAllChildren());
        observable.Subscribe(objectobserver);
        square.Side = 10;
    }

    private void ObjectStream(ICalculationObject subject)
    {
        var subjects = DbContext.GetSubjects();

        var objects = GetSubjects(subject).ToObservable();
        
    }

    private List<ICalculationObject> GetSubjects(ICalculationObject subject)
    {
        var test = 
            subject
            .GetType()
            .GetProperties()
            .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ICalculationObject)) || (p.PropertyType.GenericTypeArguments.FirstOrDefault()?.GetInterfaces().Contains(typeof(ICalculationObject)) ?? false))
            .Select(s => s.GetValue(subject))
            .Cast<ICalculationObject>()
            .ToList();

        test.AddRange(
            subject
            .GetType()
            .GetProperties()
            .Where(p => (p.PropertyType.GenericTypeArguments.FirstOrDefault()?.GetInterfaces().Contains(typeof(ICalculationObject)) ?? false))            
            .SelectMany(s => (List<ICalculationObject>)s.GetValue(subject))
            .ToList());

        test.AddRange(
            test.SelectMany(test => GetSubjects(test)));

        return test;
    }   

}