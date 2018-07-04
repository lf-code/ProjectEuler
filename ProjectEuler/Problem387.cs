using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=387
    /// </summary>
    class Problem387 : IEulerProblem
    {

        public string Solve()
        {
            //A Harshad or Niven number is a number that is divisible by the sum of its digits. 

            const int NUMBER_OF_DIGITS = 14;

            //Find the sum of the strong, right truncatable Harshad primes less than 10^14.
            long result = 0;

            //get the list of primes that are less than sqrt of the max value to be tested for primality 
            List<int> primes = AuxMethods.GetPrimes((int)BigInteger.Pow(10, NUMBER_OF_DIGITS / 2));

            List<long> rightTrucableHarshadNumbers = new List<long>();
            List<long> rightTrucableHarshadNumbersAux = new List<long>();

            //start with one-digit numbers, all are harshad numbers
            rightTrucableHarshadNumbers.AddRange(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            //(the last digit is for generating a possible prime)
            for (int digitCount = 2; digitCount < NUMBER_OF_DIGITS ; digitCount++) 
            {
                foreach (long rthNumber in rightTrucableHarshadNumbers)
                {
                    for (long d = 0; d <= 9; d++)
                    {
                        //for each number in the list, generate a new number by adding a digit to its right:
                        long n = checked(rthNumber * 10 + d);

                        //get the sum of its digits and test if it is a Harshad number 
                        long sumOfDigits = n.ToString().Select(c => long.Parse(c.ToString())).Sum();
                        bool isHarshad = n % sumOfDigits == 0;

                        if(isHarshad) 
                        {
                            //if it is a harshad number, it is 
                            //also a right truncatable Harshad number given how it was generated
                            rightTrucableHarshadNumbersAux.Add(n); //save it for next step;

                            //test that it is strong hashard 
                            //(a Harshad number that, when divided by the sum of its digits, results in a prime)
                            bool isStrongHashard = IsItPrime(primes, (long)(n / sumOfDigits));

                            if (isStrongHashard)
                            {
                                //test if theres is a digit that added to its right results in prime:
                                //thus generating a strong, right truncatable Harshad prime
                                for (long digit = 0; digit <= 9; digit++)
                                {
                                    long candidate = checked(n * 10 + digit);
                                    if (IsItPrime(primes, candidate))
                                        result = checked(result + candidate);
                                }

                            }

                        }

                    }
                }

                rightTrucableHarshadNumbers = rightTrucableHarshadNumbersAux;
                rightTrucableHarshadNumbersAux = new List<long>();
            }

            return result.ToString();
        }


        public bool IsItPrime(List<int> listOfPrimes, long p)
        {
            //it is prime if it is not divisible by any prime less than its sqrt;
            for (int i = 0; i < listOfPrimes.Count; i++)
            {
                long d = listOfPrimes[i];

                if (checked(d * d) > p) //prime greater that it sqrt, stop
                    break;

                if (p % d == 0 && p != d) //prime divides it, it is not prime.
                    return false;
            }
            return true;
        }

    }
}
