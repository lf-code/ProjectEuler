using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=321
    /// </summary>
    class Problem321 : IEulerProblem
    {

        /// <summary>
        /// Let M(n) represent the minimum number of moves/actions to completely reverse the positions of the coloured counters;
        /// A counter can move from one square to the next (slide) or can jump over another counter (hop) as long as the square next to that counter is unoccupied.
        /// Let M(n) represent the minimum number of moves/actions to completely reverse the positions of the coloured counters; 
        /// that is, move all the red counters to the right and all the blue counters to the left.
        /// </summary>
        /// <param name="n">A horizontal row comprising of 2n + 1 squares has n red counters placed at one end and n blue counters at the other end,
        /// being separated by a single empty square in the centre.</param>
        /// <returns>Let M(n) represent the minimum number of moves/actions to completely reverse the positions of the coloured counters;</returns>
        long M(long n)
        {
            //Insight: Represent particular arrangements of the coloured counters (a state) as a node in a graph where 
            //the possible moves of each state are the edges that lead to other nodes/states.
            //Use Dijkstra* algorithm to find the shortest path, that with the minimum number of moves/actions, from initial state 
            //to final state (the state with reverse the positions of the coloured counters).
            
            //This allows to find the optimal strategy for the first n, andidentify a global pattern for all solutions
            //From this pattern, we obtain that M(n) = n*n +2n; (see javascript file problem321.js)

            return checked(n*n+2*n);
        }


        public string Solve()
        {
            //GetFirstSolutions(15);

            //By printing the first 15 solutions and analysing the progression of n on Excel
            //the following rule came out: N[k] = N[k-2] + 6 * (N[k-2]-N[k-4]) - (N[k-4] -N[k-6]);

            const int NK = 40;
            long n = 0;
            long[] lastN = GetFirstSolutions(6).Reverse().ToArray(); //Fist six solutions
            int k = lastN.Length;
            long sum = lastN.Sum();

            while (k < NK)
            {
                k++;
                n = lastN[1] + 6 * (lastN[1] - lastN[3]) - (lastN[3] - lastN[5]);
                sum += n;
                Console.WriteLine($"k: {k} -> n: {n}");
                for (int i = lastN.Length - 1; i > 0; i--)
                    lastN[i] = lastN[i - 1];
                lastN[0] = n;
            }
            return sum.ToString();
        }

        public long[] GetFirstSolutions(int howMany)
        {
            //Triangle number definition: any number in the sequence T(i) = i(i+1)/2;

            int NK = howMany;
            long[] solutions = new long[howMany];
            int k = 0;
            BigInteger t = 0;
            for (BigInteger n = 1; n < long.MaxValue; n++)
            {
                //Triangle number definition: any number in the sequence T(i) = i(i+1)/2;

                //we want M(n) == Ti, that is, find some triangle number that, Ti, that equals M(n)
                //from the definitions above M(n) == Ti : n*n + 2*n == i(i+1)/2  => i^2 + i - (2*n^2+4n);

                t = -((2*n*n)+(4*n)); 

                BigInteger s = findSolution(new BigInteger(1), new BigInteger(1), t);

                if (s != 0) //found a solution such that M(n) == T(s) 
                {
                    solutions[k] = (long)n;
                    k++;
                    Console.WriteLine($"K: {k} | N: {n} | M: - | T(i) i: {s}");
                }
                if (k == NK)
                    break;
            }
            return solutions;
        }

        public BigInteger findSolution(BigInteger a, BigInteger b, BigInteger c)
        {

            // ( -b +- sqrt(b^2 - 4ac) ) / 2a


            BigInteger sqr = MySQRT(b * b - 4 * a * c);
            if (sqr == 0)
                return 0;

            BigInteger solution1 = (sqr - b) / (2*a);
            BigInteger solution2 = (sqr + b) / (2*a);

            if (solution1>0 && solution1 * (2 * a) == (sqr - b)) //sol1 is positive integer
            {
                return solution1;
            }
            else if(solution2>0 && solution2 * (2 * a) == (sqr + b))
            {
                return solution2;  //sol1 is positive integer
            }
            return 0;
        }

        static public BigInteger MySQRT(BigInteger n)
        {

            BigInteger x = n;
            BigInteger y = (BigInteger)((x + 1) / 2);
            while (y < x)
            {
                x = y;
                y = (x + (n / x)) / 2;
            }
            if (x * x != n)
                return 0;
            else
                return x;
        }

    }
}
