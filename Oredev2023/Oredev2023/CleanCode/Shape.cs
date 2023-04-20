namespace Oredev2023.CleanCode;

public abstract class Shape
{
    public abstract double Area { get; }
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double Area => Math.PI * Math.Pow(Radius, 2d);
}

public class Square : Shape
{
    public double Side { get; set; }

    public Square(double side)
    {
        Side = side;
    }

    public override double Area => Side * Side;
}


public class Rectangle : Shape
{
    public double Width { get; set; }

    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double Area => Width * Height;
}

public class Triangle : Shape
{
    public double Base { get; set; }

    public double Height { get; set; }

    public Triangle(double @base, double height)
    {
        Base = @base;
        Height = height;
    }

    public override double Area => Height * Base / 2d;
}
