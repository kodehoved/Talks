namespace Oredev2023.DDD;

public enum Shapes
{
    Square = 0,
    Rectangle = 1,
    Triangle = 2,
    Circle = 3
}

public record struct Shape(Shapes Type, double Width, double Height);

public static class ShapeArea
{
public static double GetArea(Shape shape)
{
    return shape.Type switch
    {
        Shapes.Square => shape.Width * shape.Width,
        Shapes.Rectangle => shape.Width * shape.Height,
        Shapes.Triangle => shape.Height * shape.Width / 2d,
        Shapes.Circle => shape.Width * shape.Width * Math.PI,
        _ => throw new ArgumentOutOfRangeException()
    };
}

    private static double[] multiplierPerShape = { 1d, 1d, 0.5d, Math.PI };

    public static double GetAreaFromTable(Shape shape)
    {
        return multiplierPerShape[(int)shape.Type] * shape.Width * shape.Height;
    }
}
