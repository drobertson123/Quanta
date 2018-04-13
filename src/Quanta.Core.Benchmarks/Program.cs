using Quanta.Core.Options.Model.Book;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Quanta.Core.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const long N = 500000;
            const string filename = "benchmark.htm";

            //  compute the sum by formula and print it, as a check value
            const long target = (N * (N - 1) / 2);

            Runner runner = new Runner(N);
            runner.TargetValue = target;
            for (int i = 0; i < 5; i++)
            {
                Operations<OptionBookStruct>.RunAll(runner);
            }

            string outFile = Path.Combine(@"C:\DataTest2", filename);
            string report = runner.GetReport();

            File.Delete(outFile);
            File.WriteAllText(outFile, report);
            Thread.Sleep(1000);

            ProcessStartInfo procStart = new ProcessStartInfo("Chrome", outFile);
            //Process.Start(procStart);

        }
    }
}
