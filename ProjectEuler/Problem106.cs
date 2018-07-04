using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=106
    /// </summary>
    class Problem106 : IEulerProblem
    {
        public string Solve()
        {
            int n = 12; //number of elements in [A]

            //If [A] already satisfies the second rule, 
            //we only need to consider subsets with the same number of elements.

            int k; //number of elems in both subSets, B and C.

            //For k=1, there is no need for 1st-rule testing, because,
            //by definition, elements in A are strictly increasing thus cannot be equal.

            //Insight: we don't know the value of each element, we can refer them by their position as they are strictly increasing.
            //consider the case with n= 4: e1 < e2 < e3 < e4; 
            //for k = 2 we have three possibilities: (e1,e2)|(e3,e4) , (e1,e3)|(e2,e4) , (e1,e4)|(e2,e3)
            //since e1<e3 and e2<e4 for any actual values they represent, then e1+e2 < e3+e4;
            //thus the following rule comes up: 
            //Two disjoint subsets of A, where each element is represented by its position in sorted A, 
            //sorted B = (b1,...bi..., bk), sorted C = (c1,...ci...,ck) if for every i, bi<ci, 
            //then there is no need to test, because S(B) < S(C); otherwise we need to test for 1st-rule;
        
            var countToTest = 0;
            for (k = 2; k <= (n - n % 2) / 2; k++) 
            {
                countToTest += Partial(k,n);
            }
            return countToTest.ToString();
        }

        public int Partial(int k, int n)
        {
            Random r = new Random();

            int count = 0;
            //add all possible positions in A, to the list
            List<int> lista = new List<int>();
            for (int i = 1; i <= n; i++)
                lista.Add(i);

            HashSet<long> partitions = new HashSet<long>();
            int numberOfGuesses = 300000; //number of guesses, large enough such that all possible partitions are generated at least once.
            for (var t = 0; t < numberOfGuesses; t++)
            {
                //shuffle positions, and take k*2 elements
                //part them into two sets of the same size, l1,and l2, and sort them
                List<int> aux = lista.OrderBy(x => r.Next(0, int.MaxValue)).Take(2*k).ToList();
                List<int> l1 = aux.Take(k).OrderBy(x => x).ToList();
                List<int> l2 = aux.Skip(k).OrderBy(x => x).ToList();

                //create an unique hash for such partition:
                //given k <= 6, 
                long hash = (l1[0] < l2[0] ? l1.Concat(l2) : l2.Concat(l1)).Aggregate(0, (h, x) => h * 10 + x);
    
                if (!partitions.Contains(hash)) //check if such partition was already tested as they are being generated randomly
                {
                    //Console.WriteLine(hash);

                    //sorted B = (b1,...bi..., bk), sorted C = (c1,...ci...,ck) if for every i, bi<ci 
                    bool b = true;
                    for (var j = 0; j < k; j++)
                        b = b && (l1[0] < l2[0] ? l1[j] < l2[j] : l2[j] < l1[j]);

                    //if partition does not have such property, it will be tested for 1st-rule
                    if (!b)
                        count++;

                    partitions.Add(hash);
                }
            }
            return count;
        }


    }


}
