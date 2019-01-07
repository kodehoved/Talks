using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace Polymorphism
{
    public class Test
    {
        [Params(10_000, 1_000_000)]
        public int size;

        private Base[] oo;
        private Data[] first;
        private Data[] second;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);

            oo = new Base[size];
            first = new Data[size / 2];
            second = new Data[size / 2];

            for (var i = 0; i < size; i++)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                if (i % 2 == 0)
                {
                    oo[i] = new FirstDerived { X = x, Y = y };

                    first[i / 2].X = x;
                    first[i / 2].Y = y;
                }
                else
                {
                    oo[i] = new SecondDerived { X = x, Y = y };

                    second[i / 2].X = x;
                    second[i / 2].Y = y;
                }
            }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public double VirtualDispatch()
        {
            var sum = 0d;

            for (var i = 0; i < oo.Length; i++)
            {
                sum += oo[i].Calc();
            }

            return sum;
        }

        [Benchmark]
        public double Dedicated()
        {
            var sum = 0d;

            for (var i = 0; i < first.Length; i++)
            {
                sum += Functions.CalcFirst(first[i].X, first[i].Y);
            }

            for (var i = 0; i < second.Length; i++)
            {
                sum += Functions.CalcSecond(second[i].X, second[i].Y);
            }

            return sum;
        }
    }

    public abstract class Base
    {
        public double X;
        public double Y;

        public abstract double Calc();
    }

    public class FirstDerived : Base
    {
        public override double Calc()
        {
            return Math.Sqrt(X) + Math.Sqrt(Y);
        }
    }

    public class SecondDerived : Base
    {
        public override double Calc()
        {
            return Math.Sqrt(X) * Math.Sqrt(Y);
        }
    }

    public struct Data
    {
        public double X;
        public double Y;
    }

    public static class Functions
    {
        public static double CalcFirst(double x, double y)
        {
            return Math.Sqrt(x) + Math.Sqrt(y);
        }

        public static double CalcSecond(double x, double y)
        {
            return Math.Sqrt(x) * Math.Sqrt(y);
        }
    }
}
