using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectEuler
{
    public static class AuxMethods
    {

        static public List<int> GetPrimes(int limit)
        {
            bool[] sieve = new bool[limit + 1];

            int testingLimit = (int)(Math.Ceiling(Math.Sqrt(limit)));

            int n = 0;
            for (int i = 1; i <= testingLimit; i++)
                for (int j = 1; j <= testingLimit; j++)
                {
                    n = checked(4 * i * i + j * j);
                    if (n <= limit && (n % 12 == 1 || n % 12 == 5))
                        sieve[n] = !sieve[n];

                    n = checked(3 * i * i + j * j);
                    if (n <= limit && n % 12 == 7)
                        sieve[n] = !sieve[n];

                    n = checked(3 * i * i - j * j);
                    if (n <= limit && i > j && n % 12 == 11)
                        sieve[n] = !sieve[n];

                }

            List<int> primes = new List<int>();
            primes.Add(2);
            primes.Add(3);

            for (int i = 5; i <= testingLimit; i++)
            {
                if (sieve[i])
                {
                    int k = i * i;
                    for (int j = k; j <= limit; j += k)
                    {
                        sieve[j] = false;
                    }
                }
            }

            for (int x = 5; x <= limit; x++)
            {
                if (sieve[x])
                    primes.Add(x);
            }

            return primes;
        }

    }
}
