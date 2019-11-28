using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;

namespace StructOfArrays
{
    [DisassemblyDiagnoser(printAsm: true)]
    public class Test
    {
        public Image<ARGB> ColorImageIdiomatic;
        public ARGBImage ColorImageDataDriven;

        public int Height = 2160;
        public int Width = 3840;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var random = new Random(42);

            ColorImageIdiomatic = new Image<ARGB>(Height, Width);
            ColorImageDataDriven = new ARGBImage(Height, Width);

            for (int i = 0; i < Height * Width; i++)
            {
                var a = (byte)random.Next(byte.MaxValue);
                var r = (byte)random.Next(byte.MaxValue);
                var g = (byte)random.Next(byte.MaxValue);
                var b = (byte)random.Next(byte.MaxValue);

                ColorImageIdiomatic.Pixels.Add(new ARGB { A = a, R = r, G = g, B = b });
                ColorImageDataDriven.A.Add(a);
                ColorImageDataDriven.R.Add(r);
                ColorImageDataDriven.G.Add(g);
                ColorImageDataDriven.B.Add(b);
            }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();
        }

        [Benchmark(Baseline = true)]
        public Image<Grayscale> ConvertToGrayscaleIdiomatic()
        {
            var g = new Image<Grayscale>(Height, Width);

            foreach (var p in ColorImageIdiomatic.Pixels)
            {
                g.Pixels.Add(new Grayscale { V = (byte)(0.3 * p.R + 0.59 * p.G + 0.11 * p.B) });
            }

            return g;
        }

        [Benchmark]
        public GrayscaleImage ConvertToGrayscaleDataDriven()
        {
            var g = new GrayscaleImage(Height, Width);

            for (int i = 0; i < Height * Width; i++)
            {
                g.V.Add((byte)(0.3 * ColorImageDataDriven.R[i] +
                               0.59 * ColorImageDataDriven.G[i] +
                               0.11 * ColorImageDataDriven.B[i]));
            }

            return g;
        }
    }

    public struct ARGB
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;
    }

    public struct Grayscale
    {
        public byte V;
    }

    public abstract class Area
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Area(int h, int w)
        {
            Height = h;
            Width = w;
        }
    }

    public class Image<T> : Area
    {
        public readonly List<T> Pixels;

        public Image(int h, int w) : base(h, w)
        {
            Pixels = new List<T>(h * w);
        }
    }

    public class ARGBImage : Area
    {
        public readonly List<byte> A;
        public readonly List<byte> R;
        public readonly List<byte> G;
        public readonly List<byte> B;

        public ARGBImage(int h, int w) : base(h, w)
        {
            A = new List<byte>(h * w);
            R = new List<byte>(h * w);
            G = new List<byte>(h * w);
            B = new List<byte>(h * w);
        }
    }

    public class GrayscaleImage : Area
    {
        public readonly List<byte> V;

        public GrayscaleImage(int h, int w) : base(h, w)
        {
            V = new List<byte>(h * w);
        }
    }
}
