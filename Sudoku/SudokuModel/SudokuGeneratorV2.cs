using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.SudokuModel
{
    enum BoardCharacteristic { OneSolution, ManySolutions, Error }

    public static class SudokuGeneratorV2
    {
        private static Random _rnd = new Random();

        public static byte[][] GameBoard;
        public static byte[][] CompleteBoard;
        private static Square[,] GenBoard;

        public static void Generuj(int count)
        {
            GenBoard = new Square[9, 9];

            do
            {
                GenerateCompleteBoard();
                foreach (Square sqr in GenBoard)
                {
                    sqr.Number = sqr.CompleteNumber;
                }
            }
            while (GenerateGameBoard(count) == false);

            GameBoard = new byte[9][];
            CompleteBoard = new byte[9][];
            for (int x = 0; x < 9; x++)
            {
                GameBoard[x] = new byte[9];
                CompleteBoard[x] = new byte[9];
            }

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    GameBoard[y][x] = GenBoard[y, x].Number;
                    CompleteBoard[y][x] = GenBoard[y, x].CompleteNumber;
                }
            }
        }

        private static void GenerateCompleteBoard()
        {
            Square[] board = new Square[81];
            int i = 0;
            for (byte x = 0; x < 9; x++)
            {
                for (byte y = 0; y < 9; y++)
                {
                    GenBoard[y, x] = new Square() { Y = y, X = x };
                    board[i++] = GenBoard[y, x];
                }
            }

            i = 0;
            do
            {
                if (board[i].Available == null) board[i].Available = CalcAvailableNumbers(board[i].Y, board[i].X, true);

                if (board[i].Available.Count != 0)
                {
                    int rndi = _rnd.Next(0, board[i].Available.Count);
                    byte rndv = board[i].Available.ElementAt(rndi);

                    board[i].CompleteNumber = rndv;
                    board[i].Available.Remove(rndv);
                    i++;
                }
                else
                {
                    board[i].Available = null;
                    board[i].CompleteNumber = 0;
                    i--;
                }
            }
            while (i < 81);
        }

        private static bool GenerateGameBoard(int boardNumberCount, int numbers = 81)
        {
            if (boardNumberCount == numbers) return true;

            List<Square> filled = new List<Square>();
            foreach (Square sqr in GenBoard)
            {
                if (sqr.Number != 0) filled.Add(sqr);
            }

            while (filled.Count > 0)
            {
                int rndi = _rnd.Next(0, filled.Count);
                Square rndSqr = filled.ElementAt(rndi);

                filled.Remove(rndSqr);
                rndSqr.Number = 0;

                if (TestUnique() == BoardCharacteristic.OneSolution)
                {
                    return GenerateGameBoard(boardNumberCount, numbers - 1) == true;
                }
                else
                {
                    //reversing
                    rndSqr.Number = rndSqr.CompleteNumber;
                }
            }

            return false;
        }

        private static List<byte> CalcAvailableNumbers(int by, int bx, bool complete)
        {
            byte[] possVal = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            //columns
            for (int x = 0; x < 9; x++)
            {
                byte val = complete ? GenBoard[by, x].CompleteNumber : GenBoard[by, x].Number;
                possVal[val] = 0;
            }
            //rows
            for (int y = 0; y < 9; y++)
            {
                byte val = complete ? GenBoard[y, bx].CompleteNumber : GenBoard[y, bx].Number;
                possVal[val] = 0;
            }
            //subsquare
            int subx = bx - (bx % 3);
            int suby = by - (by % 3);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    byte val = complete ? GenBoard[suby + y, subx + x].CompleteNumber : GenBoard[suby + y, subx + x].Number;
                    possVal[val] = 0;
                }
            }

            List<byte> poss = new List<byte>();
            for (int i = 0; i < 10; i++)
                if (possVal[i] != 0) poss.Add(possVal[i]);

            return poss;
        }

        private static BoardCharacteristic TestUnique()
        {
            int bx = -1;
            int by = -1;
            List<byte> bPossVal = null;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (GenBoard[y, x].Number == 0)
                    {
                        List<byte> possVal = CalcAvailableNumbers(y, x, false);
                        if (bPossVal == null || bPossVal.Count > possVal.Count)
                        {
                            bx = x;
                            by = y;
                            bPossVal = possVal;
                        }
                    }
                }
            }

            // Finished?
            if (bPossVal == null)
            {
                return BoardCharacteristic.OneSolution;
            }

            // Couldn't find a solution?
            if (bPossVal.Count == 0)
                return BoardCharacteristic.Error;

            // Try elements
            int success = 0;
            foreach (byte i in bPossVal)
            {
                GenBoard[by, bx].Number = i;

                switch (TestUnique())
                {
                    case BoardCharacteristic.OneSolution:
                        success++;
                        break;

                    case BoardCharacteristic.ManySolutions:
                        GenBoard[by, bx].Number = 0;
                        return BoardCharacteristic.ManySolutions;

                    case BoardCharacteristic.Error:
                        GenBoard[by, bx].Number = 0;
                        return BoardCharacteristic.Error;
                }

                // More than one solution found?
                if (success > 1)
                    break;
            }

            // Restore to original state.
            GenBoard[by, bx].Number = 0;

            switch (success)
            {
                case 0:
                    return BoardCharacteristic.Error;

                case 1:
                    return BoardCharacteristic.OneSolution;

                default:
                    return BoardCharacteristic.ManySolutions;
            }
        }

        class Square
        {
            internal byte X;
            internal byte Y;
            internal byte Number;
            internal byte CompleteNumber;
            internal List<byte> Available;
        }
    }
}
