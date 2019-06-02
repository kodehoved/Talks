using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdiomaticVsSimple
{
    public class Test
    {
        private static readonly int size = 100_000_000;
        private int[] array;
        private List<int> list;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);

            array = new int[size];
            list = new List<int>(size);

            for (var i = 0; i < size; i++)
            {
                var x = random.Next(100);

                array[i] = x;
                list.Add(i);
            }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public int LINQ()
        {
            var count = list.Count(i => i == 42);

            return count;
        }

        [Benchmark]
        public int Foreach()
        {
            var count = 0;

            foreach (var i in list)
            {
                if (i == 42)
                    count++;
            }

            return count;
        }

        [Benchmark]
        public int Simple()
        {
            var count = 0;

            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == 42)
                    count++;
            }

            return count;
        }
    }
}
