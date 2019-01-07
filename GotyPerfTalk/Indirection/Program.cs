using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace Indirection
{
    [DisassemblyDiagnoser(printAsm: true)]
    public class Test
    {
        [Params(1_000_000)]
        public int size;

        private SomeClass[] classes;
        private SomeStruct[] structs;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);
            var x = random.Next(100);

            classes = new SomeClass[size];
            structs = new SomeStruct[size];

            for (var i = 0; i < classes.Length; i++)
            {
                classes[i] = new SomeClass { Value = x };
            }

            for (var i = 0; i < structs.Length; i++)
            {
                structs[i] = new SomeStruct { Value = x };
            }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public int ReferenceType()
        {
            var sum = 0;

            for (var i = 0; i < classes.Length; i++)
            {
                sum += classes[i].Value;
            }

            return sum;
        }

        [Benchmark]
        public int ValueType()
        {
            var sum = 0;

            for (var i = 0; i < structs.Length; i++)
            {
                sum += structs[i].Value;
            }

            return sum;
        }
    }

    public class SomeClass
    {
        public int Value;
    }

    public struct SomeStruct
    {
        public int Value;
    }
}
