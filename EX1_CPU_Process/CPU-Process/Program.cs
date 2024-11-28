using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 1 && int.TryParse(args[0], out int iterations))
        {
            Console.WriteLine("Running intensive calculations...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            IntensiveCalculation(iterations);
            stopwatch.Stop();
            Console.WriteLine($"Time for {iterations} iterations: {stopwatch.Elapsed.TotalMilliseconds} ms");

        }
        else
        {
            ShowErrorMessage();
            return;
        }

        void ShowErrorMessage()
        {
            Console.WriteLine("Invalid input. Please provide a single numeric argument.");
        }
    }

    static void IntensiveCalculation(int iterations)
    {
        double result = 0;
        for (int i = 0; i < iterations; i++)
        {
            result += Math.Sqrt(i) * Math.Tan(i) * Math.Exp(i*0.05);
        }
    }
}