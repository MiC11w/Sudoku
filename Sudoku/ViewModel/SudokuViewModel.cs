using GalaSoft.MvvmLight;
using Sudoku.SudokuModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sudoku.ViewModel
{
    public enum GameState { GameOn, Pause, Finished }
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    /// 
    public class SudokuViewModel : ViewModelBase
    {
        private FieldViewModel _selectedField = null;
        private MyStopwatch _stopwatch;
        private DispatcherTimer dt;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SudokuViewModel()
        {
            Sudoku = new SudokuGame();
            SudokuBoard = new FieldViewModel[9, 9];
            Sudoku.GameFinished += Sudoku_GameFinished;

            ViewModelLocator locator = Application.Current.Resources["Locator"] as ViewModelLocator;
            locator.Settings.PropertyChanged += Settings_PropertyChanged;
            
            TimeSpan prevGameTime;
            AppSettings.TryGetSetting("PreviousGameTime", out prevGameTime);
            _stopwatch = new MyStopwatch(prevGameTime);

            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dt.Tick += dt_Tick;
            Start();

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    SudokuBoard[y, x] = new FieldViewModel(Sudoku.GameBoard[y, x], new Color() { A = 255, R = 255, G = 200, B = 100 });
                }
            }
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowErrors")
            {
                foreach (FieldViewModel f in SudokuBoard)
                {
                    f.SetForeState();
                }
            }
        }

        void Sudoku_GameFinished()
        {
            _stopwatch.Stop();
            SudokuState = GameState.Finished;
            dt.Stop();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            RaisePropertyChanged("GameTime");
        }

        public void SaveGame()
        {
            if (SudokuState != GameState.Finished)
            {
                AppSettings.StoreSetting("PreviousGameTime", _stopwatch.Elapsed);
                Sudoku.SaveGame();
            }
            else
            {
                AppSettings.StoreSetting("PreviousGame", null);
                AppSettings.StoreSetting("PreviousGameTime", new TimeSpan());
            }
        }

        public void Start()
        {
            _stopwatch.Start();
            SudokuState = GameState.GameOn;
            dt.Start();
        }

        public void Pause()
        {
            _stopwatch.Stop();
            SudokuState = GameState.Pause;
            dt.Stop();
        }

        private GameState _sudokuState;
        public GameState SudokuState 
        {
            get
            {
                return _sudokuState;
            }
            set
            {
                if (value != _sudokuState)
                {
                    _sudokuState = value;
                    RaisePropertyChanged("SudokuState");
                }
            }
        }

        public void Solve()
        {
            Sudoku.Solve();
        }

        public void AddRandomNumber()
        {
            Sudoku.FillRandomField();
        }

        public void Refresh()
        {
            Sudoku.Regenerate();
            _stopwatch.Reset();
            Start();
        }

        public string GameTime
        {
            get
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds);
                return ts.ToString(@"mm\:ss");
            }
        }

        private SudokuGame _sudoku;
        public SudokuGame Sudoku
        {
            get
            {
                return _sudoku;
            }
            set
            {
                _sudoku = value;
                RaisePropertyChanged("Sudoku");
            }
        }

        public void SetNumber(int? number)
        {
            if (_selectedField != null)
            {
                Sudoku.AddNumber(_selectedField.Model.X, _selectedField.Model.Y, number);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">1-9</param>
        /// <param name="y">1-9</param>
        public void FocusField(FieldViewModel f)
        {
            _selectedField = f;

            int x = f.Model.X - 1;
            int y = f.Model.Y - 1;
            Unfocus();

            ViewModelLocator locator = Application.Current.Resources["Locator"] as ViewModelLocator;

            if (f.Model.Editable == true && locator.Settings.HighlightRowColumn == true)
            {
                for (int iy = 0; iy < 9; iy++)
                {
                    SudokuBoard[iy, x].BackState = FieldViewModel.BackgroundState.Semiactive;
                }

                for (int ix = 0; ix < 9; ix++)
                {
                    SudokuBoard[y, ix].BackState = FieldViewModel.BackgroundState.Semiactive;
                }
            }
            else if (f.Model.Editable == false && locator.Settings.HighlightNumbers == true)
            {
                foreach (FieldViewModel field in SudokuBoard)
                {
                    if (f.Number == field.Number) field.BackState = FieldViewModel.BackgroundState.Semiactive;
                }
            }

            SudokuBoard[y, x].BackState = FieldViewModel.BackgroundState.Active;
        }

        public void Unfocus()
        {
            foreach (FieldViewModel field in SudokuBoard)
            {
                field.BackState = FieldViewModel.BackgroundState.Normal;
            }
        }

        public List<List<FieldViewModel>> AllSubsquares()
        {
            List<List<FieldViewModel>> outList = new List<List<FieldViewModel>>();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    List<FieldViewModel> subsqr = Subsquare(SudokuBoard[x * 3, y * 3], SudokuBoard);
                    outList.Add(subsqr);
                }
            }

            return outList;
        }

        private FieldViewModel[,] _sudokuBoard;
        public FieldViewModel[,] SudokuBoard
        {
            get
            {
                return _sudokuBoard;
            }
            set
            {
                _sudokuBoard = value;
                RaisePropertyChanged("SudokuBoard");
            }
        }

#region private methods

        private List<FieldViewModel> Subsquare(FieldViewModel f, FieldViewModel[,] b)
        {
            List<FieldViewModel> output = new List<FieldViewModel>();
            int startX = f.Model.X - ((f.Model.X - 1) % 3);
            int startY = f.Model.Y - ((f.Model.Y - 1) % 3);

            for (int x = startX; x < startX + 3; x++)
            {
                for (int y = startY; y < startY + 3; y++)
                {
                    output.Add(b[y - 1, x - 1]);
                }
            }

            return output;
        }

#endregion
    }
}