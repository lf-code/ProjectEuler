using System;

namespace ProjectEuler
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();

            IEulerProblem problem = new Problem96();

            w.Start();
            string answer = problem.Solve();
            w.Stop();

            Console.WriteLine($"The answer to {problem.GetType().ToString()} is: {answer}");
            Console.WriteLine($"It took {(w.ElapsedMilliseconds / 1000f):F2} seconds to solve.");
            Console.Read();
        }
    }

    public interface IEulerProblem
    {
        string Solve();
    }

}
