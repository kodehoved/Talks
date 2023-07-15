using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var b = new Benchmarks();
b.RefactoredPlayer.HandleInputs();
b.AlternativePlayer.HandleInputs();
Console.WriteLine($"Position ({b.RefactoredPlayer.X},{b.RefactoredPlayer.Y})");
Console.WriteLine($"Position ({b.AlternativePlayer.X},{b.AlternativePlayer.Y})");

var summary = BenchmarkRunner.Run<Benchmarks>();

public class Benchmarks
{
    private const int Size = 1_000_000;
    public readonly RefactoringToClasses.Refactored.Player RefactoredPlayer;
    public readonly RefactoringToClasses.Alternative.Player AlternativePlayer;

    public Benchmarks()
    {
        var r = new Random(42);
        RefactoredPlayer = new RefactoringToClasses.Refactored.Player();
        AlternativePlayer = new RefactoringToClasses.Alternative.Player();
        for (int i = 0; i < Size; i++)
        {
            var value = r.Next(4);
            switch (value)
            {
                case 0: 
                    RefactoredPlayer.InputLeft(); 
                    AlternativePlayer.InputLeft(); 
                    break;
                case 1: 
                    RefactoredPlayer.InputRight();
                    AlternativePlayer.InputRight(); 
                    break;
                case 2: 
                    RefactoredPlayer.InputUp();
                    AlternativePlayer.InputUp(); 
                    break;
                case 3: 
                    RefactoredPlayer.InputDown();
                    AlternativePlayer.InputDown(); 
                    break;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public (int X, int Y) Refactored()
    {
        RefactoredPlayer.HandleInputs();

        return (RefactoredPlayer.X, RefactoredPlayer.Y);
    }

    [Benchmark]
    public (int X, int Y) Alternative()
    {
        AlternativePlayer.HandleInputs();

        return (AlternativePlayer.X, AlternativePlayer.Y);
    }
}