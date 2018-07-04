using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=96
    /// </summary>
    class Problem96 : IEulerProblem
    {
        public string Solve()
        {
            //Get problem input sudoku puzzles:
            string[] rawData = File.ReadAllLines("../../../Problem96InputData.txt");
            List<Puzzle> puzzles = new List<Puzzle>();
            int i = 1;
            while(i < rawData.Length)
            {
                string puzzleString = "";
                for (int j = 0; j < 9; j++)
                    puzzleString += rawData[i++].Trim();
                puzzles.Add(new Puzzle(puzzleString));
                i++; //ignore "Grid##" line 
            }

            //Solve all puzzles and construct the ans:
            int answer = 0;
            foreach(Puzzle puzzle in puzzles)
            {
                puzzle.Solve();
                answer += puzzle.GetTopLeftStringAsNumber();
            }

            return answer.ToString();
        }


        class Puzzle
        {
            public Place[] Places { get; set; }

            public IEnumerable<Place> GetRow(int row) =>
                 Places.Where(place => place.Row == row);

            public IEnumerable<Place> GetColumn(int column) =>
                Places.Where(place => place.Column == column);

            public IEnumerable<Place> GetBox(int box) =>
                Places.Where(place => place.Box == box);

            // Iterates all for all rows, columns, and boxes.
            public IEnumerable<IEnumerable<Place>> AllSets()
            {
                for (int i = 0; i < 9; i++)
                {
                    yield return GetRow(i);
                    yield return GetColumn(i);
                    yield return GetBox(i);
                }
            }

            //Constructs the puzzle from a string of 81 chars containing the values for each place ('0' for an empty place)
            public Puzzle(string data)
            {
                Places = new Place[9 * 9];
                for (int i = 0; i < 9 * 9; i++)
                {
                    int value = int.Parse(data[i].ToString());
                    Places[i] = new Place(i, value);
                }
            }

            //Checks whether a place can hold a given value according to the rules of sudoku.
            public bool CanPlaceHoldValue(Place place, int value)
            {
                if (place.Value != 0)
                    return false; //not an empty place

                //No place in the same row, column or box can have the same value.
                return !GetRow(place.Row).Union(GetColumn(place.Column)).Union(GetBox(place.Box)).Any(p => p.Value == value);
            }

            /// <summary>
            /// Logic rule 1 for solving the puzzle 
            /// </summary>
            /// <returns>Whether this rule made any progress in solving the puzzle</returns>
            public bool ApplyRuleOne()
            {
                // Rule One: each set (row, column or box) must have all the different values once, thus if there is 
                //         only one place in the set that can contain a given value, that's it.

                bool wereChangesMade = false;
                for (int value = 1; value <= 9; value++)
                {
                    foreach (IEnumerable<Place> set in AllSets())
                    {
                        int count = 0;
                        Place candidatePlace = null;
                        foreach (Place place in set)
                        {
                            if (CanPlaceHoldValue(place, value))
                            {
                                count++; //count the number of places in the set that can hold the value
                                candidatePlace = place; //save the last place, in case it turns out to be unique
                            }
                        }
                        if (count == 1) //if only place can hold the value, set the value in that place.
                        {
                            candidatePlace.Value = value;
                            wereChangesMade = true;
                        }
                    }
                }
                return wereChangesMade;
            }

            /// <summary>
            /// Logic rule 2 for solving the puzzle 
            /// </summary>
            /// <returns>Whether this rule made any progress in solving the puzzle</returns>
            public bool ApplyRuleTwo()
            {
                // Rule Two: each place must have a value, if there is only one compatible value for that place, that's it.

                bool wereChangesMade = false;
                foreach (Place place in Places)
                {
                    int count = 0;
                    int candidateValue = -1;
                    for (int value = 1; value <= 9; value++)
                    {
                        if (CanPlaceHoldValue(place, value))
                        {
                            count++; //Count the possible values;
                            candidateValue = value;
                        }
                    }
                    if(count == 1) //If there's only one possible value, that's it:
                    {
                        place.Value = candidateValue;
                        wereChangesMade = true;
                    }
                }
                return wereChangesMade;
            }

            /// <summary>
            /// Tries to solve the puzzle in its current state
            /// </summary>
            /// <returns>Whether it was able to solve the puzzle or not</returns>
            public bool Solve()
            {
                //Keep applying the two logic rules while any of them is making progress:
                while (ApplyRuleOne() || ApplyRuleTwo());

                if (IsSolved())
                    return true;

                //If the logic rules alone weren't able to solve the puzzle,
                //try and guess a value for an empty place among the possible values. 

                //Save puzzle before guessing:
                int[] savedData = Save();

                //Get an empty place, guess a possible value and and try to solve from that state:
                Place placeToGuess = Places.Where(p => p.Value == 0).First();
                for (int value = 1; value <= 9; value++)
                {
                    if(CanPlaceHoldValue(placeToGuess,value))
                    {
                        placeToGuess.Value = value;
                        if (Solve())
                            return true;

                        //if not successful, restore puzzle:
                        Restore(savedData);
                    }
                }
                
                return false;
            }

            //Returns an array representing the current state of the puzzle (the value in each place)
            public int[] Save()
            {
                int[] savedData = new int[Places.Length];
                for (int i = 0; i < Places.Length; i++)
                    savedData[i] = Places[i].Value;
                return savedData;
            }

            //Restores the puzzle state, from a previously saved state
            public void Restore( int[] savedData)
            {
                for (int i = 0; i < Places.Length; i++)
                    Places[i].Value = savedData[i];
            }

            //Returns whether the puzzle's state corresponds to a solution for the puzzle.
            public bool IsSolved()
            {
                foreach(IEnumerable<Place> set in AllSets())
                {
                    //each set must contain 9 distinct values from 1 to 9;
                    IEnumerable<int> values = set.Select(place => place.Value);
                    if (!(values.Distinct().Count() == 9 && values.Min() == 1 && values.Max() == 9))
                        return false;
                }
                return true;
            }

            //Aux method required to construct the final answer for the Problem.
            public int GetTopLeftStringAsNumber()
            {
                return int.Parse(Places[0].Value.ToString() 
                    + Places[1].Value.ToString() 
                    + Places[2].Value.ToString());
            }
        }

        class Place {
            public int Index { get; set; } //Index on the grid 0 to 80, numbered left-to-right, top-to-bottom;
            public int Row { get; } //row number, top-to-bottom;
            public int Column { get;  } //column number, left-to-right;
            public int Box { get; } //3 by 3 box that must contain unique values, numbered left-to-right, top-to-bottom
            public int Value { get; set; }

            public Place(int index, int value)
            {
                Index = index;
                Row = Index/9;
                Column = Index%9;
                Box = (Row/3)*3+Column/3;
                Value = value;
            }
        }
    }
}
