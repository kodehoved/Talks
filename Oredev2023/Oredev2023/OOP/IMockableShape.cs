namespace Oredev2023.OOP;

public interface IMockableShape
{
    double GetArea();
}

public class MockableCircle : IMockableShape
{
    private readonly double radius;

    public MockableCircle(double radius)
    {
        this.radius = radius;
    }

    public double GetArea()
    {
        return Math.PI * Math.Pow(radius, 2d);
    }
}

public class MockableSquare : IMockableShape
{
    private readonly double side;

    public MockableSquare(double side)
    {
        this.side = side;
    }

    public double GetArea()
    {
        return side * side;
    }
}


public class MockableRectangle : IMockableShape
{
    private readonly double width;

    private readonly double height;

    public MockableRectangle(double width, double height)
    {
        this.width = width;
        this.height = height;
    }

    public double GetArea()
    {
        return width * height;
    }
}

public class MockableTriangle : IMockableShape
{
    private readonly double @base;

    private readonly double height;

    public MockableTriangle(double @base, double height)
    {
        this.@base = @base;
        this.height = height;
    }

    public double GetArea()
    {
        return height * @base / 2d;
    }
}
