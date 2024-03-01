namespace ReactiveProgramming
{
    public interface ICube : ICalculationObject
    {
        int EdgeLength { get; set; }
        int Height { get; set; }
        Square Face { get; }
        int SurfaceArea { get; set; }
        int Volume { get; set; }
    }
}