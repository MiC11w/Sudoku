using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Sudoku.SudokuModel
{
    public class Field : ViewModelBase
    {
        public Field(int x, int y)
        {
            X = x;
            Y = y;
            _number = null;
        }

        /// <summary>
        /// For sudoku solver
        /// </summary>
        /// <param name="f"></param>

        public void Clear()
        {
            Number = null;
            Editable = true;
        }


        public void SetNumber(int? number)
        {
            if (Editable == true)
            {
                Number = number;
            }
        }

        internal void SetNumber(int? number, bool editable)
        {
            Number = number;
            Editable = editable;
        }

        internal void SetCorrectNumber(int correctNumber)
        {
            CorrectNumber = correctNumber;
        }

        public bool IsCorrect()
        {
            if (Editable == false) return true;
            else if (CorrectNumber == Number) return true;
            else return false;
        }

        private int? _number;
        public int? Number
        {
            get
            {
                return _number;
            }
            private set
            {
                if (_number != value)
                {
                    _number = value;
                    RaisePropertyChanged("Number");
                }
            }
        }

        private int _correctNumber;
        public int CorrectNumber
        {
            get
            {
                return _correctNumber;
            }
            private set
            {
                if (_correctNumber != value)
                {
                    _correctNumber = value;
                    RaisePropertyChanged("CorrectNumber");
                }
            }
        }

        bool _editable = true;
        public bool Editable
        {
            get
            {
                return _editable;
            }
            private set
            {
                if (_editable != value)
                {
                    _editable = value;
                    RaisePropertyChanged("Editable");
                }
            }

        }

        private int _x;
        public int X
        {
            get
            {
                return _x;
            }
            private set
            {
                if (_x != value)
                {
                    _x = value;
                    RaisePropertyChanged("X");
                }
            }
        }

        private int _y;
        public int Y
        {
            get
            {
                return _y;
            }
            private set
            {
                if(_y != value)
                {
                    _y = value;
                    RaisePropertyChanged("Y");
                }
            }
        }
    }
}
