using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=232
    /// </summary>
    class Problem232 : IEulerProblem
    {
        public string Solve()
        {

            const int G = 100;

            //winProb[x1,x2] holds the probability that player 2 wins, 
            //measured at the beginning of player 2's turn, when player i is Xi points away from the goal G;  
            decimal[,] winProb = new decimal[G + 1, G + 1];

            //initialize winProb (default zero has meaning, thus initialize to min value)
            for (int g1 = 0; g1 <= G; g1++)
                for (int g2 = 0; g2 <= G; g2++)
                    winProb[g1, g2] = decimal.MinValue;

            //Player 1 plays first, thus if he reaches the goal, player 2 loses no matter what
            winProb[0, 0] = 0.0M;
            for (int i = 1; i <= G; i++)
            {
                winProb[0, i] = 0.0M;
                winProb[i, 0] = 1.0M; //if player 2 reaches the goal first, he wins;
            }

            //calculate all probabilities:
            for (int g1 = 1; g1 <= G; g1++)
                for (int g2 = 1; g2 <= G; g2++)
                    winProb[g1, g2] = ProbOptimalStrategy(g1, g2, ref winProb);


            //WHEN PLAYER 2 PLAYS HE KNOWS WHAT HAPPENED IN PLAYER 1's TURN!!
            //PROBEXACT measeares probability of winning at the beggining of player 2's turn!!
            //thus the game begins with 1/2 * [99,100] (player 1 heads of fisrt turn) and 1/2 * [100,100] player 1 tails on its first turn!!

            //As winProb measures the probability of player 2 win at the beginning of player 2's turn, and the game starts with player 1's turn
            //we need to adjust to this fact: 
            //the game for player 2 begins with 1/2 * [G-1,G] (player 1 scores on first turn) and 1/2 * [G,G] (player 1 fails to score on 1st turn)
            decimal ans =  0.5M * winProb[G - 1, G] + 0.5M * winProb[G, G];

            return string.Format("{0:F8}", ans);

        }

        /// <summary>
        /// Calculates the probability that player 2 wins the game, for a given starting position,
        /// if he always chooses his best strategy.
        /// Calculated at the beginning of player 2's turn, for the position [pointToGoP1, pointToGoP2].
        /// Insight: prob[state j] = Sum i ( probabity of ending up in state i * prob[state i])
        /// Calculate probability recursively.
        /// </summary>
        /// <param name="pointToGoP1">points that player 1 has to score to win the game.</param>
        /// <param name="pointsToGoPlayer2">points that player 2 has to score to win the game.</param>
        /// <param name="winProb">Calculates the probability that player 2 wins the game</param>
        /// <returns></returns>
        public static decimal ProbOptimalStrategy(int pointToGoP1, int pointsToGoPlayer2, ref decimal[,] winProb)
        {

            //player 1 reached his goal, no chance of player two winning
            if (pointToGoP1 <= 0) 
                return 0.0M;

            //player 2 reached his goal, he wins.
            if (pointsToGoPlayer2 <= 0)
                return 1.0M;
            
            //if the probability for a given state has been calculated before, return it 
            if (winProb[pointToGoP1, pointsToGoPlayer2] > decimal.MinValue)
                return winProb[pointToGoP1, pointsToGoPlayer2];

            //Calculate the probability of the optimal strategy
            decimal probBestStrategy = 0.0M;

            //If some strategy Tmax provides more points (in case of scoring) than pointsToGoPlayer2, 
            //than player 2 will never choose T > Tmax has it reduces the prob of scoring with no aditional gain; 
            int T = 0;
            int pointsToScoreForT = 0;
            do
            {
                T++; //next strategy;
                pointsToScoreForT = (int)Math.Ceiling(Math.Pow(2, T - 1)); //points player 2 would score, given strategy T;

                //he chooses a positive integer T and tosses the coin T times

                decimal probT = 0.0M; //probability of player 2 winning, if he chooses strategy T

                decimal probScoringP1 = 1.0M / 2.0M; //probabily of scoring for player 1 (toss comes up Heads); 
                decimal probScoringP2 = 1.0M / (decimal)Math.Pow(2, T); //probabily of scoring for player 2, given strategy T (T tosses, all come up Heads); 


                //For a give strategy four scenarios may happen before player 2 plays again:
                //P2 scores P1 fails, P2 scores P1 scores, P2 fails P1 scores, P2 fails, P1 fails.

                // probT = Sum i ( probabity of ending up scenario i * probability of winning the game in scenario i)

                if (pointsToGoPlayer2 - pointsToScoreForT <= 0)
                    probT += probScoringP2 * 1.0M; // Scenario 1a: player 2 scores more points that he needs to win the game, player 1 doesn't play next
                else
                {
                    //Scenario 1b: Player 2 scores, Player 1 also scores next
                    probT += probScoringP2 * probScoringP1 * ProbOptimalStrategy(pointToGoP1 - 1, pointsToGoPlayer2 - pointsToScoreForT, ref winProb);

                    ///Scenario 2: Player 2 scores, Player 1 doesn't score next
                    probT += probScoringP2 * (1 - probScoringP1) * ProbOptimalStrategy(pointToGoP1, pointsToGoPlayer2 - pointsToScoreForT, ref winProb);
                }

                //Scenario 3: Player 2 does not score, Player 1 scores next
                probT += (1 - probScoringP2) * probScoringP1 * ProbOptimalStrategy(pointToGoP1 - 1, pointsToGoPlayer2, ref winProb);

                //Scenario 4: neither player scores
                // ProbT(final) = ProbT(initial) + (ProbP2fails * ProbP1fails) * Prob(final) ==> ProbT(final) = Prob(initial)/(1-(ProbP2fails * ProbP1fails));
                probT = probT / (1.0M - ((1.0M - probScoringP2) * (1.0M - probScoringP1)));

                if (probT > probBestStrategy) //save probT if T is best strategy so far
                    probBestStrategy = probT;

            } while (pointsToScoreForT < pointsToGoPlayer2);

            return probBestStrategy;
        }

    }
}
