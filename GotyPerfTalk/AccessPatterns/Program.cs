using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;

namespace AccessPatterns
{
    [DisassemblyDiagnoser(printAsm: true)]
    public class Test
    {
        private byte[] data;
        private int[] ordered_index;
        private int[] random_index;

        [Params(10_000, 10_000_000)]
        public int size;

        [GlobalSetup]
        public void GlobalSetup()
        {
            data = new byte[size];
            ordered_index = new int[size];

            var random = new Random(42);

            random.NextBytes(data);

            for (var i = 0; i < size; i++)
            {
                ordered_index[i] = i;
            }

            random_index = ordered_index.OrderBy(_ => Guid.NewGuid()).ToArray();
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public int Ordered()
        {
            var sum = 0;

            for (var i = 0; i < data.Length; i++)
            {
                sum += data[ordered_index[i]];
            }

            return sum;
        }

        [Benchmark]
        public int Random()
        {
            var sum = 0;

            for (var i = 0; i < data.Length; i++)
            {
                sum += data[random_index[i]];
            }

            return sum;
        }
    }
}
