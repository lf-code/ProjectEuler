using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=189
    /// </summary>
    class Problem189 : IEulerProblem
    {
        public string Solve()
        {
            int numberOfRows = 15; //has to be odd.

            //Insight: starting at the top triangle, and adding each row one by one,
            //each new row has only to be compatible with the last row added.
            //the number of different triangles that will have the new row as their last row is the sum of
            //all triangles (with one less row) that have a last row compatible with the new one.

            //map of different possibilities for a row of a given size -> number of different triangles that have that row as their last row.
            Dictionary<string, long> row = new Dictionary<string, long>(); 
            row.Add("r", 1);
            row.Add("g", 1);
            row.Add("b", 1);

            for (int i = 0; i < numberOfRows; i++)
            {
                Dictionary<string, long> newRow = new Dictionary<string, long>();
                foreach (string s in row.Keys)
                {
                    if (i % 2 == 0)
                        newRow.Add(s, 0);
                    else //every other row, increase the row size by one
                    {
                        newRow.Add(s + "r", 0);
                        newRow.Add(s + "g", 0);
                        newRow.Add(s + "b", 0);
                    }
                }

                foreach (string s0 in row.Keys.ToArray())
                    foreach (string s1 in newRow.Keys.ToArray())
                    {
                        int k = 0;
                        bool isCompatible = true;
                        while(isCompatible && k<s0.Length)
                        {
                            isCompatible &= s0[k] != s1[k];
                            if(s1.Length>s0.Length)
                                isCompatible &= s0[k] != s1[k+1];
                            k++;
                        }
                        if (isCompatible)
                            newRow[s1] += row[s0];
                    }
                row = newRow;
            }

            long ans = row.Values.Sum();
            return ans.ToString();
        }

    }
}
