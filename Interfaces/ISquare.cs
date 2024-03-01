namespace ReactiveProgramming
{
    public interface ISquare : ICalculationObject
    {
        int Area { get; set; }
        double Diagonal { get; set; }
        int Perimeter { get; set; }
        int Side { get; }
    }
}