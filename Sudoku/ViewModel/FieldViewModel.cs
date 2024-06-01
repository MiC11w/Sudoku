using GalaSoft.MvvmLight;
using Sudoku.SudokuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sudoku.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        public enum BackgroundState { Active, Semiactive, Normal };
        public enum ForegroundState { Static, Editable, Incorrect };
        private Color _color;

        public FieldViewModel(Field f, Color color)
        {
            Model = f;
            Model.PropertyChanged += _field_PropertyChanged;

            _color = color;
            _backState = BackgroundState.Normal;
            SetForeState();
        }

        public Field Model { get; private set; }

        private BackgroundState _backState;
        public BackgroundState BackState
        {
            get
            {
                return _backState;
            }
            set
            {
                if (_backState != value)
                {
                    _backState = value;
                    RaisePropertyChanged("BackState");
                    RaisePropertyChanged("BackColorBrush");
                    RaisePropertyChanged("MarginColorBrush");
                }
            }
        }


        private ForegroundState _foreState;
        public ForegroundState ForeState
        {
            get
            {
                return _foreState;
            }
            set
            {
                if (_foreState != value)
                {
                    _foreState = value;
                    RaisePropertyChanged("ForeState");
                    RaisePropertyChanged("ForeColorBrush");
                }
            }
        }

        public SolidColorBrush MarginColorBrush
        {
            get
            {
                switch (BackState)
                { 
                    case BackgroundState.Normal:
                    case BackgroundState.Semiactive:
                        return new SolidColorBrush(Colors.White);
                    default:
                        return new SolidColorBrush(Colors.DarkGray);
                }
            }
        }

        public SolidColorBrush BackColorBrush
        {
            get
            {
                switch (BackState)
                {
                    case BackgroundState.Normal:
                        {
                            SolidColorBrush backBrush = new SolidColorBrush(_color);

                            int sx = Model.X - 1;
                            sx = sx - (sx % 3);
                            sx /= 3;
                            int sy = Model.Y - 1;
                            sy = sy - (sy % 3);
                            sy /= 3;

                            if ((sx + sy) % 2 == 0)
                            {
                                backBrush.Opacity = 0.7;
                            }
                            else
                            {
                                backBrush.Opacity = 0.55;
                            }

                            return backBrush;
                        }
                    case BackgroundState.Semiactive:
                        {
                            SolidColorBrush backBrush = new SolidColorBrush(_color);
                            backBrush.Opacity = 0.2;
                            return backBrush;
                        }
                    default:
                        return new SolidColorBrush(Colors.White);
                }
            }
        }

        public SolidColorBrush ForeColorBrush
        {
            get
            {
                switch (ForeState)
                {
                    case ForegroundState.Static:
                        return new SolidColorBrush(IncreaseBrightness(Colors.DarkGray, 0.8));
                    case ForegroundState.Incorrect:
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }
        }

        void _field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Number")
            {
                SetForeState();
            }
            else if (e.PropertyName == "Editable")
            {
                if (Model.Editable == true) ForeState = ForegroundState.Editable;
                else ForeState = ForegroundState.Static;
            }
        }

        public void SetForeState()
        {
            ViewModelLocator locator = Application.Current.Resources["Locator"] as ViewModelLocator;

            if (Model.Editable == false)
            {
                ForeState = ForegroundState.Static;
            }
            else if (Model.Number == Model.CorrectNumber || locator.Settings.ShowErrors == false)
            {
                ForeState = ForegroundState.Editable;
            }
            else
            {
                ForeState = ForegroundState.Incorrect;
            }
            RaisePropertyChanged("Number");
        }

        public string Number
        {
            get
            {
                if (Model.Number != null)
                {
                    return Model.Number.ToString();
                }
                else return "";
            }
        }

        private Color IncreaseBrightness(Color c, double v)
        {
            return Color.FromArgb(c.A, (byte)(c.R * v), (byte)(c.G * v), (byte)(c.B * v));
        }
    }
}
