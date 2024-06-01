using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.SudokuModel
{
    public class SudokuBoard : ViewModelBase
    {
        public SudokuBoard()
        {
            _board = new Field[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    _board[y, x] = new Field(x + 1, y + 1);
                }
            }
        }

        Field[,] _board;
        public Field[,] GameBoard
        {
            get
            {
                return _board;
            }
            private set
            {
                _board = value;
                RaisePropertyChanged("Board");
            }
        }

        public List<Field> Subsquare(Field f, Field[,] b)
        {
            List<Field> output = new List<Field>();
            int startX = f.X - ((f.X - 1) % 3);
            int startY = f.Y - ((f.Y - 1) % 3);

            for (int x = startX; x < startX + 3; x++)
            {
                for (int y = startY; y < startY + 3; y++)
                {
                    output.Add(b[y - 1, x - 1]);
                }
            }

            return output;
        }

        public List<List<Field>> AllSubsquares(Field[,] b)
        {
            List<List<Field>> outList = new List<List<Field>>();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    List<Field> subsqr = Subsquare(b[x * 3, y * 3], b);
                    outList.Add(subsqr);
                }
            }

            return outList;
        }
    }
}
