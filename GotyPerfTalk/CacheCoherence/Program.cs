using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

namespace CacheCoherence
{
    public class Test
    {
        [Params(1, 2, 4, 8)]
        public int threads;

        private static readonly int size = 100_000_000;
        private double[] data = new double[size];
        private double[] result_per_thread;

        private readonly object token = new object();

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);

            result_per_thread = new double[threads];

            for (var i = 0; i < data.Length; i++)
                data[i] = random.Next(1000);
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public double SimpleLoop()
        {
            double result = 0;

            for (var i = 0; i < data.Length; i++)
                result += Math.Sqrt(data[i]);

            return result;
        }

        [Benchmark]
        public double Broken()
        {
            double result = 0;

            Parallel.For(0, threads, index =>
            {
                var start = index * data.Length / threads;
                var stop = (index + 1) * data.Length / threads;

                for (var i = start; i < stop; i++)
                    result += Math.Sqrt(data[i]);
            });

            return result;
        }

        [Benchmark]
        public double Locking()
        {
            double result = 0;

            Parallel.For(0, threads, index =>
            {
                var start = index * data.Length / threads;
                var stop = (index + 1) * data.Length / threads;

                for (var i = start; i < stop; i++)
                    lock (token) result += Math.Sqrt(data[i]);
            });

            return result;
        }

        [Benchmark]
        public double LockFree()
        {
            double result = 0;

            Parallel.For(0, threads, index =>
            {
                var start = index * data.Length / threads;
                var stop = (index + 1) * data.Length / threads;
                for (var i = start; i < stop; i++)
                    result_per_thread[index] += Math.Sqrt(data[i]);
            });

            for (var i = 0; i < result_per_thread.Length; i++)
                result += result_per_thread[i];

            return result;
        }

        [Benchmark]
        public double LocalStorage()
        {
            double result = 0;

            Parallel.For(0, threads, index =>
            {
                var local = 0d;
                var start = index * data.Length / threads;
                var stop = (index + 1) * data.Length / threads;

                for (var i = start; i < stop; i++)
                    local += Math.Sqrt(data[i]);

                result_per_thread[index] = local;
            });
            
            for (var i = 0; i < result_per_thread.Length; i++)
                result += result_per_thread[i];

            return result;
        }


    }
}
