using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.SudokuModel
{
    enum BoardStatus { Solvable, NotEnoughNumbers, Error }

    public class SudokuGame : SudokuBoard
    {
        Random _rnd = new Random();

        public SudokuGame()
        {
            //restoring data of previous game
            Byte[][] prevGameBoard;
            Byte[][] prevGameCompleteBoard;
            bool[][] prevGameEditableFields;
            AppSettings.TryGetSetting("PreviousGame", out prevGameBoard);
            AppSettings.TryGetSetting("PreviousGameCompleteBoard", out prevGameCompleteBoard);
            AppSettings.TryGetSetting("PreviousGameEditableFields", out prevGameEditableFields);

            if (prevGameBoard != null) //restoring previous game
            {
                SetBoard(prevGameBoard, prevGameCompleteBoard, prevGameEditableFields);
            }
            else //new game
            {
                GenerateBoard();
            }
        }

        #region public

        public delegate void GameFinishedEventHandler();
        public event GameFinishedEventHandler GameFinished;

        public void SaveGame()
        {
            byte[][] completeGameBoard;
            byte[][] gameBoard;
            bool[][] prevGameEditableFields;

            GetByteArrays(out gameBoard, out completeGameBoard, out prevGameEditableFields);
            AppSettings.StoreSetting("PreviousGame", gameBoard);
            AppSettings.StoreSetting("PreviousGameCompleteBoard", completeGameBoard);
            AppSettings.StoreSetting("PreviousGameEditableFields", prevGameEditableFields);
        }

        public void Regenerate()
        {
            ClearGameBoard();
            GenerateBoard();
        }

        public void AddNumber(int x, int y, int? number)
        {
            Field f = GameBoard[y - 1, x - 1];
            f.SetNumber(number);
            CheckForWin();
        }

        public void Solve()
        {
            foreach (Field f in GetEmptyFields())
            {
                int corrNumber = f.CorrectNumber;
                f.SetNumber(corrNumber);
            }
            CheckForWin();
        }

        public void FillRandomField()
        {
            Field rf = GetRandomEmtpyField();
            if (rf != null)
            {
                int corrNumber = rf.CorrectNumber;
                rf.SetNumber(corrNumber);
            }
            CheckForWin();
        }

        #endregion

        private void GetByteArrays(out Byte[][] gameBoard, out Byte[][] completeGameBoard, out bool[][] editableFields)
        {
            Byte[][] gb = new Byte[9][];
            Byte[][] cb = new Byte[9][];
            bool[][] ef = new bool[9][];

            for (int x = 0; x < 9; x++)
            {
                gb[x] = new byte[9];
                cb[x] = new byte[9];
                ef[x] = new bool[9];
            }

            foreach (Field f in GameBoard)
            {
                int x = f.X - 1;
                int y = f.Y - 1;

                if (f.Number != null) gb[y][x] = (byte)f.Number;
                cb[y][x] = (byte)f.CorrectNumber;

                ef[y][x] = f.Editable;
            }

            gameBoard = gb;
            completeGameBoard = cb;
            editableFields = ef;
        }

        /// <summary>
        /// Fills board with numbers until board is solvable or speciefied count of fields is reached
        /// </summary>
        /// <param name="fieldCount">81 > fieldCount >= 17</param>
        private void GenerateBoard()
        {
            int diff;
            AppSettings.TryGetSetting("Difficulty", out diff);
            SudokuGeneratorV2.Generuj(diff);
            byte[][] gboard = SudokuGeneratorV2.GameBoard;
            byte[][] cboard = SudokuGeneratorV2.CompleteBoard;

            SetBoard(gboard, cboard);
        }

        private void SetBoard(byte[][] gameBoard, byte[][] completeBoard, bool[][] editableFields = null)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (gameBoard[y][x] != 0)
                    {
                        bool editable = false;
                        if (editableFields != null) editable = editableFields[y][x];
                        GameBoard[y, x].SetNumber(gameBoard[y][x], editable);
                    }
                    else GameBoard[y, x].SetNumber(null, true);

                    GameBoard[y, x].SetCorrectNumber(completeBoard[y][x]);
                }
            }
        }

        private bool CheckForWin()
        {
            foreach (Field f in GameBoard)
            {
                if (f.IsCorrect() == false) return false;
            }

            if (GameFinished != null) GameFinished();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>number of filled fields on board</returns>
        private int FilledFields()
        {
            int count = 0;
            foreach (Field f in GameBoard)
            {
                if (f.Number != null)
                {
                    count++;
                }
            }

            return count;
        }

        private void ClearGameBoard()
        {
            foreach (Field f in GameBoard)
            {
                f.SetNumber(null, true);
            }
        }

        private Field GetRandomEmtpyField()
        {
            List<Field> emptyFields = GetEmptyFields();
            if (emptyFields.Count == 0) return null;

            int idx = _rnd.Next(0, emptyFields.Count);
            return emptyFields.ElementAt(idx);
        }

        private List<Field> GetEmptyFields()
        {
            List<Field> outList = new List<Field>();
            foreach (Field f in GameBoard)
            {
                if (f.Number == null) outList.Add(f);
            }
            return outList;
        }
    }
}
