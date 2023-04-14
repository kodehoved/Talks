namespace Oredev2023;

using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Oredev2023.DDD;
using Oredev2023.OOP;

public class Program
{
    public static void Main()
    {
        Output();

        var summary = BenchmarkRunner.Run<Benchmarks>();
    }

    public static void Output()
    {
        var x = new Benchmarks();

        Console.WriteLine($"OOP            {x.Oop()}");
        Console.WriteLine($"OOP LINQ       {x.OopLinq()}");
        Console.WriteLine($"OOP MOCK       {x.OopMockable()}");
        Console.WriteLine($"DDD            {x.Ddd()}");
        Console.WriteLine($"DDD TABLE      {x.DddTable()}");
        Console.WriteLine($"DDD UNROLL     {x.DddTableUnrolled()}");
        Console.WriteLine($"OPTIMIZED      {x.Optimized()}");
        Console.WriteLine($"OPTIMIZED WIDE {x.Optimized()}");
    }
}

public class Benchmarks
{
    private const int Size = 1_000;
    private const int Threads = 8;

    private readonly List<OOP.Shape> oopData = new(Size);

    private readonly List<IMockableShape> oopMockableData = new(Size);

    private readonly DDD.Shape[] dddData = new DDD.Shape[Size];

    private readonly DDD.Shape[] dddDataTable = new DDD.Shape[Size];

    private readonly double[] widths = new double[Size];

    private readonly double[] heights = new double[Size];

    private readonly double[] multipliers = new double[Size];

    private readonly double[] threadResult = new double[Threads];


    [Benchmark(Baseline = true)]
    public double Oop()
    {
        var areaSum = 0d;
        foreach (var s in oopData)
        {
            areaSum += s.GetArea();
        }

        return areaSum;
    }

    [Benchmark]
    public double OopLinq()
    {
        return oopData.Sum(s => s.GetArea());
    }

    [Benchmark]
    public double OopMockable()
    {
        var areaSum = 0d;
        foreach (var s in oopData)
        {
            areaSum += s.GetArea();
        }

        return areaSum;
    }

    [Benchmark]
    public double Ddd()
    {
        var areaSum = 0d;
        for (int i = 0; i < Size; i++)
        {
            areaSum += ShapeArea.GetArea(dddData[i]);
        }

        return areaSum;
    }

    [Benchmark]
    public double DddTable()
    {
        var areaSum = 0d;
        for (int i = 0; i < Size; i++)
        {
            areaSum += ShapeArea.GetAreaFromTable(dddDataTable[i]);
        }

        return areaSum;
    }

    // [Benchmark]
    public double DddTableUnrolled()
    {
        var areaSum = 0d;
        for (int i = 0; i < Size; i += 4)
        {
            areaSum += ShapeArea.GetAreaFromTable(dddDataTable[i]);
            areaSum += ShapeArea.GetAreaFromTable(dddDataTable[i + 1]);
            areaSum += ShapeArea.GetAreaFromTable(dddDataTable[i + 2]);
            areaSum += ShapeArea.GetAreaFromTable(dddDataTable[i + 3]);
        }

        return areaSum;
    }

    [Benchmark]
    public double Optimized()
    {
        var areaSum = 0d;

        var count = Vector<double>.Count;
        for (int i = 0; i <= Size - count; i += count)
        {
            var v1 = new Vector<double>(new ReadOnlySpan<double>(widths, i, count));
            var v2 = new Vector<double>(new ReadOnlySpan<double>(heights, i, count));
            var prod = v1 * v2;
            var v3 = new Vector<double>(new ReadOnlySpan<double>(multipliers, i, count));
            var res = prod * v3;

            areaSum += Vector.Sum(res);
        }

        return areaSum;
    }

    [Benchmark]
    public double OptimizedWide()
    {
        var areaSum = 0d;

        var count = Vector<double>.Count;
        Parallel.For(0, Threads, index =>
        {
            var localSum = 0d;
            var start = index + widths.Length / Threads;
            var stop = (index + 1) + widths.Length / Threads;
            for (int i = start; i <= stop - count; i += count)
            {
                var v1 = new Vector<double>(new ReadOnlySpan<double>(widths, i, count));
                var v2 = new Vector<double>(new ReadOnlySpan<double>(heights, i, count));
                var prod = v1 * v2;
                var v3 = new Vector<double>(new ReadOnlySpan<double>(multipliers, i, count));
                var res = prod * v3;

                localSum += Vector.Sum(res);
            }

            threadResult[index] = localSum;
        });

        for(var i = 0; i < threadResult.Length; i++)
        {
            areaSum += threadResult[i];
        }

        return areaSum;
    }

    public Benchmarks()
    {
        var rng = new Random(42);
        for (int i = 0; i < Size; i++)
        {
            var t = rng.Next(4);
            switch (t)
            {
                case 0:
                    {
                        var radius = rng.NextDouble() * 100d + 100d;
                        oopData.Add(new Circle(radius));
                        oopMockableData.Add(new MockableCircle(radius));
                        dddData[i] = new DDD.Shape
                        {
                            Type = Shapes.Circle,
                            Width = radius,
                            Height = 0d
                        };
                        dddDataTable[i] = new DDD.Shape
                        {
                            Type = Shapes.Circle,
                            Width = radius,
                            Height = radius
                        };
                        widths[i] = radius;
                        heights[i] = radius;
                        multipliers[i] = Math.PI;
                        break;
                    }
                case 1:
                    {
                        var width = rng.NextDouble() * 100d + 100d;
                        oopData.Add(new Square(width));
                        oopMockableData.Add(new MockableSquare(width));
                        dddData[i] = new DDD.Shape
                        {
                            Type = Shapes.Square,
                            Width = width,
                            Height = 0d
                        };
                        dddDataTable[i] = new DDD.Shape
                        {
                            Type = Shapes.Square,
                            Width = width,
                            Height = width
                        };
                        widths[i] = width;
                        heights[i] = width;
                        multipliers[i] = 1d;
                        break;
                    }
                case 2:
                    {
                        var width = rng.NextDouble() * 100d + 100d;
                        var height = rng.NextDouble() * 100d + 100d;
                        oopData.Add(new Rectangle(width, height));
                        oopMockableData.Add(new MockableRectangle(width, height));
                        dddData[i] = new DDD.Shape
                        {
                            Type = Shapes.Rectangle,
                            Width = width,
                            Height = height
                        };
                        dddDataTable[i] = new DDD.Shape
                        {
                            Type = Shapes.Rectangle,
                            Width = width,
                            Height = height
                        };
                        widths[i] = width;
                        heights[i] = height;
                        multipliers[i] = 1d;
                        break;
                    }
                case 3:
                    {
                        var @base = rng.NextDouble() * 100d + 100d;
                        var height = rng.NextDouble() * 100d + 100d;
                        oopData.Add(new Triangle(@base, height));
                        oopMockableData.Add(new MockableTriangle(@base, height));
                        dddData[i] = new DDD.Shape
                        {
                            Type = Shapes.Triangle,
                            Width = @base,
                            Height = height
                        };
                        dddDataTable[i] = new DDD.Shape
                        {
                            Type = Shapes.Triangle,
                            Width = @base,
                            Height = height
                        };
                        widths[i] = @base;
                        heights[i] = height;
                        multipliers[i] = 0.5d;
                        break;
                    }
            }
        }
    }
}
