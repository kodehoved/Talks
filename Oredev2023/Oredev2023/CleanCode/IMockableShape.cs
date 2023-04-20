namespace Oredev2023.CleanCode;

public interface IMockableShape
{
    double Area { get; }
}

public class MockableCircle : IMockableShape
{
    public double Radius { get; set; }

    public MockableCircle(double radius)
    {
        Radius = radius;
    }

    public double Area =>Math.PI * Math.Pow(Radius, 2d);
}

public class MockableSquare : IMockableShape
{
    public double Side { get; set; }

    public MockableSquare(double side)
    {
        Side = side;
    }

    public double Area =>Side * Side;
}


public class MockableRectangle : IMockableShape
{
    public double Width { get; set; }

    public double Height { get; set; }

    public MockableRectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Area => Width * Height;
}

public class MockableTriangle : IMockableShape
{
    public double Base { get; set; }

    public double Height { get; set; }

    public MockableTriangle(double @base, double height)
    {
        Base = @base;
        Height = height;
    }

    public double Area => Height * Base / 2d;
}
