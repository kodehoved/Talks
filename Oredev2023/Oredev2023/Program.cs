namespace Oredev2023;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Oredev2023.CleanCode;
using Oredev2023.Alternative;
using System.Numerics;


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

        Console.WriteLine($"Clean Code           :{x.CleanCode()}");
        Console.WriteLine($"Clean Code LINQ      :{x.CleanCodeLinq()}");
        Console.WriteLine($"Clean Code Mockable  :{x.CleanCodeMockable()}");
        Console.WriteLine($"No Polymorphism      :{x.NoPolymorphism()}");
        Console.WriteLine($"Table Driven         :{x.TableDriven()}");
        Console.WriteLine($"Table Driven Unrolled:{x.TableDrivenUnrolled()}");
        Console.WriteLine($"Optimized            :{x.Optimized()}");
        Console.WriteLine($"Optimized Wide       :{x.OptimizedWide()}");
    }
}

public class Benchmarks
{
    private const int Size = 1_000_000;
    private const int Threads = 32;

    private readonly List<CleanCode.Shape> cleanCodeData = new(Size);

    private readonly List<IMockableShape> cleanCodeMockableData = new(Size);

    private readonly Alternative.Shape[] noPolymorphismData = new Alternative.Shape[Size];

    private readonly Alternative.Shape[] tableDrivenData = new Alternative.Shape[Size];

    private readonly double[] widths = new double[Size];

    private readonly double[] heights = new double[Size];

    private readonly double[] multipliers = new double[Size];

    private readonly double[] threadResult = new double[Threads];


    [Benchmark(Baseline = true)]
    public double CleanCode()
    {
        var areaSum = 0d;
        foreach (var s in cleanCodeData)
        {
            areaSum += s.Area;
        }

        return areaSum;
    }

    [Benchmark]
    public double CleanCodeLinq()
    {
        return cleanCodeData.Sum(s => s.Area);
    }

    //[Benchmark]
    public double CleanCodeMockable()
    {
        var areaSum = 0d;
        foreach (var s in cleanCodeMockableData)
        {
            areaSum += s.Area;
        }

        return areaSum;
    }

    [Benchmark]
    public double NoPolymorphism()
    {
        var areaSum = 0d;
        for (int i = 0; i < Size; i++)
        {
            areaSum += Alternative.Shape.GetArea(noPolymorphismData[i]);
        }

        return areaSum;
    }

    [Benchmark]
    public double TableDriven()
    {
        var areaSum = 0d;
        for (int i = 0; i < Size; i++)
        {
            areaSum += Alternative.Shape.GetAreaFromTable(tableDrivenData[i]);
        }

        return areaSum;
    }

    //[Benchmark]
    public double TableDrivenUnrolled()
    {
        var areaSum = 0d;
        for (int i = 0; i < Size; i += 4)
        {
            areaSum += Alternative.Shape.GetAreaFromTable(tableDrivenData[i]);
            areaSum += Alternative.Shape.GetAreaFromTable(tableDrivenData[i + 1]);
            areaSum += Alternative.Shape.GetAreaFromTable(tableDrivenData[i + 2]);
            areaSum += Alternative.Shape.GetAreaFromTable(tableDrivenData[i + 3]);
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
            var start = index * widths.Length / Threads;
            var stop = (index + 1) * widths.Length / Threads;
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

        for (var i = 0; i < threadResult.Length; i++)
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
                        cleanCodeData.Add(new Circle(radius));
                        cleanCodeMockableData.Add(new MockableCircle(radius));
                        noPolymorphismData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Circle,
                            Width = radius,
                            Height = 0d
                        };
                        tableDrivenData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Circle,
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
                        cleanCodeData.Add(new Square(width));
                        cleanCodeMockableData.Add(new MockableSquare(width));
                        noPolymorphismData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Square,
                            Width = width,
                            Height = 0d
                        };
                        tableDrivenData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Square,
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
                        cleanCodeData.Add(new Rectangle(width, height));
                        cleanCodeMockableData.Add(new MockableRectangle(width, height));
                        noPolymorphismData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Rectangle,
                            Width = width,
                            Height = height
                        };
                        tableDrivenData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Rectangle,
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
                        cleanCodeData.Add(new Triangle(@base, height));
                        cleanCodeMockableData.Add(new MockableTriangle(@base, height));
                        noPolymorphismData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Triangle,
                            Width = @base,
                            Height = height
                        };
                        tableDrivenData[i] = new Alternative.Shape
                        {
                            Type = Alternative.Shape.Shapes.Triangle,
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
