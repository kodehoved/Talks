using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<Benchmarks>();

public class Benchmarks
{
    private const int Size = 1_000_000;
    private readonly int[] Data1 = new int[Size];
    private readonly int[] Data2 = new int[Size];

    public Benchmarks()
    {
        var r = new Random(42);
        for (int i = 0; i < Size; i++)
        {
            var value = r.Next(100);
            Data1[i] = value;
            Data2[i] = value;
        }
    }

    [Benchmark(Baseline = true)]
    public int[] EveryElement()
    {
        for (int i = 0; i < Data1.Length; i++)
        {
            Data1[i] *= 3;
            //Data1[i] = Calculation(Data1[i]);
        }

        return Data1;
    }

    [Benchmark]
    public int[] Every16thElement()
    {
        for (int i = 0; i < Data2.Length; i += 16)
        {
            Data2[i] *= 3;
            //Data2[i] = Calculation(Data2[i]);
        }

        return Data2;
    }

    private static int Calculation(int i) => (int)(Math.Sqrt(i.GetHashCode() * Math.PI) + Math.Cbrt(i.GetHashCode() / (i % 4 + 3)));
}