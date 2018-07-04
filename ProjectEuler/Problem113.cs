using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;
namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=113
    /// </summary>
    class Problem113 : IEulerProblem
    {
        public string Solve()
        {
            // Non Bouncy number: a number which is either increasing (256689) or decreasing (97741)

            /* no need to save the numbers, we only need to know how many increasing/decreasing numbers exist
            *  with a given last digit. we add digits one by one ( till M digits), and the count of increasing numbers 
            *  ending in the added digit is equal to sum of increasing numbers with less one digit and ending in 
            *  a digit that is less than the added digit (analogous for decreasing).
            *  increasing[4](3-digit) == increasing[4](2-digit) + increasing[3](2-digit) + increasing[2](2-digit) + increasing[1](2-digit) + increasing[0](2-digit)
            *  [(144,244,334,444),(134,234,334),(124,224),(114)] <- [14,24,34,44] + [13,23,33] + [12,22] + [11];
            */

            const int MAX_NUMBER_OF_DIGITS = 100; //max number of digits ( exponent of 10^100 )
            const int DIFFERENT_DIGITS = 10; //number of diferent possibilities for last digit of a number: 0-9
            long[] decrCount = new long[DIFFERENT_DIGITS]; //count of decreasing numbers ending in 0-9;
            long[] incrCount = new long[DIFFERENT_DIGITS]; //count of increasing numbers ending in 0-9;

            
            //Final NonBouncyNumbers count:
            long nonBouncyNumbersTotalCount = 0;

            //first digit: one digit numbers are both increasing and deacreasing
            for (int firstDigit = 1; firstDigit <= 9; firstDigit++)
            {
                decrCount[firstDigit] = 1;
                incrCount[firstDigit] = 1;
                nonBouncyNumbersTotalCount++;
            }

            for (int n = 2; n <= MAX_NUMBER_OF_DIGITS; n++) //add digits one by one, till M
            {
                for (int addedDigit = 0; addedDigit <= 9; addedDigit++)
                    decrCount[addedDigit] = decrCount.Where((value, index) => index >= addedDigit).Sum();

                for (int addedDigit = 9; addedDigit >= 0; addedDigit--)
                    incrCount[addedDigit] = incrCount.Where((value, index) => index <= addedDigit).Sum();

                nonBouncyNumbersTotalCount += incrCount.Sum() + decrCount.Sum();

                //there are numbers that are both increasing and decreasing, i.e, the numbers where all digits are equal;
                //these numbers should be subtracted to the final count of nonBouncy numbers, as they are being tracked in both inc and decr counts.
                nonBouncyNumbersTotalCount -= 9; //all numbers with n identical digits, 1-9;
            }

            return nonBouncyNumbersTotalCount.ToString();
        }
    
    }
}
