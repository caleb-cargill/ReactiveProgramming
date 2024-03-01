namespace ReactiveProgramming
{
    public interface IRubixCube : ICalculationObject
    {
        List<Cube> Cubes { get; }
        int EdgeLength { get; set; }
        int Height { get; set; }
        int SurfaceArea { get; set; }
        int Volume { get; set; }
    }
}