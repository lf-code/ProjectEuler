using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProjectEuler
{
    /// <summary>
    /// https://projecteuler.net/problem=220
    /// </summary>
    class Problem220 : IEulerProblem
    {
        public string Solve()
        {
            //By drawing the algoritm (Dn is an exotic drawing known as the Heighway Dragon of order n)
            //and analysing the results (see javascript: Problem 220.js)
            //the following rule comes up: if we start a 0,0 and with the direction 'd' and move 'k' steps of length 'l' according to the rules of Dn
            //we will end up in the same point as starting at 0,0 with direction [ i % 4 ] and moving [k/(4^i)] steps of lenght [l*[2^(i)] according to the rules of the same Dn 
            //for any i such that k % (4^i) = 0; (greater i means less steps to calculate)

            //MyPoint ans = MoveCursor(10, 500, new MyPoint(0, 0, 0, 1));

            //input:
            long k = 1000000000000L;
            int depth = 50;

            //find largest i such that k % (4 ^ i) = 0:
            int i = 0;
            while (k%(int)Math.Pow(4, i)==0 )
                i++;
            i--;

            //recalculate input according to rules:
            int k2 = (int)(k / (long)Math.Pow(4, i));
            Direction dir2 = (Direction)(i % 4);
            int length2 = (int)Math.Pow(2, i);

            //calculate ans:
            //MyPoint ans = MoveCursor(depth, k2, new MyPoint(0, 0, dir2, length2));

            //Faster Execution:
            string ans = MoveCursorFaster(depth, k2, (byte)dir2, length2);

            return ans.ToString();
        }


        /// <summary>
        /// represents position of the computer cursor
        /// </summary>
        public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 };

        public struct MyPoint
        {
            public int x;
            public int y;
            public Direction d; 
            public int length; //the length of each step

            public MyPoint(int x, int y, Direction d, int length)
            {
                this.x = x;
                this.y = y;
                this.d = d;
                this.length = length;
            }

            /// <summary>
            /// moves the cursor in its direction, by its length
            /// </summary>
            public void Move()
            {
                switch (d)
                {
                    case Direction.Up: y += length; break;
                    case Direction.Right: x += length; break;
                    case Direction.Down: y -= length; break;
                    case Direction.Left: x -= length; break;
                }
            }

            public void RotateLeft()
            {
                switch (d)
                {
                    case Direction.Up: d = Direction.Left; break;
                    case Direction.Right: d = Direction.Up; break;
                    case Direction.Down: d = Direction.Right; break;
                    case Direction.Left: d = Direction.Down; break;
                }
            }
            public void RotateRight()
            {
                switch (d)
                {
                    case Direction.Up: d = Direction.Right; break;
                    case Direction.Right: d = Direction.Down; break;
                    case Direction.Down: d = Direction.Left; break;
                    case Direction.Left: d = Direction.Up; break;
                }
            }

            public override string ToString()
            {
                return $"{x},{y}";
            }

        }

        public enum InstructionValue { F, L, R, a, b };

        public struct MyInstruction
        {
            public InstructionValue instruction;
            public byte depth;

            public MyInstruction(InstructionValue instruction, byte depth)
            {
                this.instruction = instruction;
                this.depth = depth;
            }
        }

        MyPoint MoveCursor(int maxDepth, int numMoves, MyPoint startingPoint)
        {

            int k = 0; //number of moves 

            MyPoint p = startingPoint; //[x,y]; 32x, 

            /* From Problem 220: 
             * Let D0 be the two-letter string "Fa". For n≥1, derive Dn from Dn-1 by the string-rewriting rules:
             * "a" → "aRbFR"
             * "b" → "LFaLb"
             * Thus, D0 = "Fa", D1 = "FaRbFR", D2 = "FaRbFRRLFaLbFR", and so on.
             * "These strings can be interpreted as instructions to a computer graphics program, 
             * with "F" meaning "draw forward one unit", "L" meaning "turn left 90 degrees", 
             * "R" meaning "turn right 90 degrees", and "a" and "b" being ignored. 
             * The initial position of the computer cursor is (0,0), pointing up towards (0,1)."*/

            //Insight: It is not necessary to know the full instruction string before we start moving.
            //every time we encounter 'a' or 'b' we replace it, adding to each instruction a parameter 'depth'
            //that represents how many replacements took place that originate such instruction.
            //Thus: thus every time we pop an instruction a[i] from the stack, we push instructions 
            //R[i+1], F[i+1], b[i+1], R[i+1], a[i+1] (inverted order of "aRbFR")
            //onto the stack if i < maxDepth, otherwise we ignore the instruction and move on to the next one. (similar for 'b' instructions).

            Stack<MyInstruction> s = new Stack<MyInstruction>();
            s.Push(new MyInstruction(InstructionValue.a, 0));  //a
            s.Push(new MyInstruction(InstructionValue.F, 0));  //f

            while (k < numMoves)
            {
                //pop next instruction:
                MyInstruction i = s.Pop();

                if (i.instruction == InstructionValue.F)
                {
                    p.Move();
                    k++; //count move
                }
                else if (i.instruction == InstructionValue.L) 
                {
                    p.RotateLeft(); //rotate left
                }
                else if (i.instruction == InstructionValue.R)
                {
                    p.RotateRight(); //rotate right
                }
                else
                {
                    //should a|b be replaced or ignored?
                    //if depth < maxdepth, replace and go deeper one level, otherwise ignore
                    if (i.depth < maxDepth)
                    {
                        //TOO SLOW
                        //string replaceString = i.instruction == InstructionValue.a ? "aRbFR" : "LFaLb";
                        //foreach (string newInst in replaceString.Reverse().Select(x => x.ToString()))
                        //{
                        //    InstructionValue auxInst = (InstructionValue)Enum.Parse(typeof(InstructionValue), newInst, false);
                        //    s.Push(new MyInstruction(auxInst, (byte)(i.depth + 1)));
                        //}

                        if (i.instruction == InstructionValue.a)
                        {
                            //RFbRa 
                            s.Push(new MyInstruction(InstructionValue.R, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.F, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.b, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.R, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.a, (byte)(i.depth + 1)));

                        }
                        else
                        {
                            //bLaFL 
                            s.Push(new MyInstruction(InstructionValue.b, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.L, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.a, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.F, (byte)(i.depth + 1)));
                            s.Push(new MyInstruction(InstructionValue.L, (byte)(i.depth + 1)));
                        }
                    }
                }
            }

            return p; //return the point we reached after moving 'numMoves' times
        }


        /// <summary>
        /// Faster implementation without struct, but with the same logic.
        /// </summary>
        string MoveCursorFaster(int maxDepth, int numMoves, byte direction, int length)
        {
            if (maxDepth > byte.MaxValue)
                throw new Exception("maxDepth must be less than 256");

            int k = 0; //number of moves 

            int x = 0;
            int y = 0;

            //instruction byte => i%5 = instruction (a=0,b=1,F=2, L=3, R=4) i/5 = depth
            Stack<byte> s = new Stack<byte>();
            s.Push(0);  //a,0
            s.Push(2);  //f,0

            while (k < numMoves)
            {
                //pop next instruction:
                byte i = s.Pop();

                if (i % 5 == 2) // 'F' 
                {

                    switch (direction)
                    {
                        case 0: y += length; break;
                        case 1: x += length; break;
                        case 2: y -= length; break;
                        case 3: x -= length; break;
                    }
                    k++;
                    //k++; //count move
                }
                else if (i % 5 > 2) // 'L' or 'R'
                {
                    direction = (byte)((16 + direction + ((i % 5) == 3 ? -1 : 1)) % 4);
                }
                else
                {
                    //should a|b be replaced or ignored?
                    //if depth < maxdepth, replace and go deeper one level, otherwise ignore
                    if (i / 5 < maxDepth)
                    {
                        //i%5 (a = 0, b = 1, F = 2, L = 3, R = 4)   i/5 = depth
                        uint dp = (uint)((i / 5) + 1);
                        if (i % 5 == 0)
                        {
                            //RFbRa -> 42140
                            s.Push((byte)(dp * 5 + 4));
                            s.Push((byte)(dp * 5 + 2));
                            s.Push((byte)(dp * 5 + 1));
                            s.Push((byte)(dp * 5 + 4));
                            s.Push((byte)(dp * 5 + 0));

                        }
                        else
                        {
                            //bLaFL -> 13023
                            s.Push((byte)(dp * 5 + 1));
                            s.Push((byte)(dp * 5 + 3));
                            s.Push((byte)(dp * 5 + 0));
                            s.Push((byte)(dp * 5 + 2));
                            s.Push((byte)(dp * 5 + 3));
                        }

                    }
                }
            }

            return $"{x},{y}";
        }
    }
}
