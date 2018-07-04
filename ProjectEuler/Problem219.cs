using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=219
    /// </summary>
    class Problem219 : IEulerProblem
    {
        public string Solve()
        {
            //Insight: to increase the size of the set, we must sacrifice one element of the group.
            //that element will be used as a prefix for the added elements.
            //sacrifice "00" -> add "001", "000"
            //added cost: (c + 1) + (c + 4) - c where c is the cost of the sacrificed elem
            //the added cost, c+5, is decreases with c, so pick the elem with the least cost.

            //we don't need to go one by one, we may group elements by their cost, and take all elements with the least cost
            // at once, say i elems with cost c, and we have C(n+i) => C(n) + i * 5;  and List[c+1] += i; List[c+4] += i;

            //key: cost Value: how many elements in the set have such cost
            SortedList<long,long> list = new SortedList<long, long>();
            list.Add(1, 1);
            list.Add(4,1);

            long N = 1000000000;

            long cost = 5;
            long i = 2;
            while ( i < N)
            {
                var kvp = list.ElementAt(0);

                long nElements = 0;
                if (kvp.Value < N - i)
                {
                    nElements = kvp.Value;
                    list.RemoveAt(0);
                }
                else
                {
                    nElements = N - i;
                    list[kvp.Key] -= N - i;
                }

                foreach (long delta in new long[] { 1, 4 })
                    if (!list.TryAdd(kvp.Key + delta, nElements))
                        list[kvp.Key + delta] += nElements;


                cost += (kvp.Key + 5) * nElements;
                i += nElements;
                
                Console.WriteLine($"Cost({i}) = {cost} ");
            }

            return cost.ToString();
        }

        private void FirstSolutions(int n)
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(4);

            int lastCost = 0;
            for (int i = 3; i <= n; i++)
            {
                int min = list.Min();
                list.Remove(min);
                list.Add(min + 1);
                list.Add(min + 4);
                int cost = list.Sum();
                Console.WriteLine($"Cost({i}) = {cost} | Added cost: {cost - lastCost}");
                lastCost = cost;
            }
        }
    }
}
