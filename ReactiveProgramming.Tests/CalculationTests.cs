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
    }
}