using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace ArrayLayout
{
    [DisassemblyDiagnoser(printAsm: true)]
    public class Test
    {
        [Params(1_000, 10_000)]
        public int size;

        private int[,] matrix;
        private int[] vector;

        [GlobalSetup]
        public void GlobalSetup()
        {
            matrix = new int[size, size];
            vector = new int[size * size];

            var random = new Random(42);

            var i = 0;
            for (var y = 0; y < size; y++)
                for (var x = 0; y < size; y++)
                {
                    var n = random.Next(100);
                    matrix[x, y] = n;
                    vector[i++] = n;
                }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public int ColFirst()
        {
            var sum = 0;

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    sum += matrix[x, y];

            return sum;
        }

        [Benchmark]
        public int RowFirst()
        {
            var sum = 0;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    sum += matrix[x, y];

            return sum;
        }

        [Benchmark]
        public int Vector()
        {
            var sum = 0;

            for (int x = 0; x < vector.Length; x++)
                sum += vector[x];

            return sum;
        }
    }
}
