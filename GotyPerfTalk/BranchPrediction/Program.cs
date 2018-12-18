using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;

namespace BranchPrediction
{
    public class Test
    {
        [Params(1, 10, 50, 90, 99)]
        public int percent_negative;

        private static readonly int size = 1_000_000;
        private int[] data = new int[size];
        private int[] sorted = new int[size];
        private int[] output = new int[size];

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);

            for (var i = 0; i < data.Length; i++)
            {
                var n = random.Next(1000);
                data[i] = (random.Next(100) + 1 < percent_negative) ? -n : n;
            }

            Array.Copy(data, sorted, data.Length);
            Array.Sort(sorted);
            Array.Clear(output, 0, output.Length);
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public int Abs()
        {
            for (var i = 0; i < data.Length; i++)
            {
                var x = data[i];
                output[i] = (x < 0) ? -x : x;
            }

            return data.Length;
        }

        [Benchmark]
        public int AbsNoBranch()
        {
            for (var i = 0; i < data.Length; i++)
            {
                var x = data[i];
                var m = x >> (8 * sizeof(int) - 1);
                output[i] = ((x ^ m) - m);
            }

            return data.Length;
        }

        [Benchmark]
        public int AbsSorted()
        {
            for (var i = 0; i < sorted.Length; i++)
            {
                var x = sorted[i];
                output[i] = (x < 0) ? -x : x;
            }

            return sorted.Length;
        }

        [Benchmark]
        public int AbsNoBranchSorted()
        {
            for (var i = 0; i < sorted.Length; i++)
            {
                var x = sorted[i];
                var m = x >> (8 * sizeof(int) - 1);
                output[i] = (x < 0) ? -x : x;
            }

            return sorted.Length;
        }
    }
}
