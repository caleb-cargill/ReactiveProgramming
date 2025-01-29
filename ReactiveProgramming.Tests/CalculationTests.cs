using System.Reactive.Linq;

namespace ReactiveProgramming.Tests
{
    [TestFixture]
    public class CalculationTests
    {

        [Test]
        public void Test_SimpleCalculation()
        {
            // Arrange
            var subject = Source.GetSquare();
            var subjects = new List<ICalculationObject>() { subject };
            var calc = new CalculatorBase(subjects);
            var calculation = DbContext.GetCalculations().First(c => c.Name == "CalculateArea");

            // Act
            var result = calc.Calculate(subject, calculation);
            
            // Assert
            Assert.AreEqual("25", result);  
        }

        [Test]
        public void Test_ObjectNodes_CalculationsTriggeredByPropertySet_SquareSide10_ResultsAllCorrectValues()
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

            // Assert
            Assert.IsTrue(rb.Cubes.All(c => c.Face.Side == 10));
            Assert.IsTrue(rb.Cubes.All(c => c.Face.Area == 100));
            Assert.IsTrue(rb.Cubes.All(c => c.Face.Perimeter == 40));
            Assert.IsTrue(rb.Cubes.All(c => c.Face.Diagonal == (Math.Sqrt(200))));
            Assert.IsTrue(rb.Cubes.All(c => c.Volume == 1000));
            Assert.IsTrue(rb.Cubes.All(c => c.SurfaceArea == 600));
            Assert.IsTrue(rb.Cubes.All(c => c.EdgeLength == 120));
            Assert.IsTrue(rb.Cubes.All(c => c.Height == 10));
            Assert.That(rb.Volume, Is.EqualTo(8000));
            Assert.That(rb.SurfaceArea, Is.EqualTo(2400));
            Assert.That(rb.Height, Is.EqualTo(20));
            Assert.That(rb.EdgeLength, Is.EqualTo(240));
        }
    }       
}