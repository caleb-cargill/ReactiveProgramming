namespace ReactiveProgramming;

public static class Source
{
    public static Square GetSquare()
        => new Square() { Side = 5 };

    public static Cube GetCube()
        => new Cube() { Face = GetSquare() };

    public static RubixCube GetRubixCube()
        => new RubixCube() { Cubes = new List<Cube>() { GetCube(), GetCube(), GetCube(), GetCube() } };
}
