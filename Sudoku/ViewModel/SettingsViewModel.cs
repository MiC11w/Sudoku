
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public bool HighlightNumbers
        {
            get
            {
                bool highlightNumbers;
                AppSettings.TryGetSetting("HighlightNumbers", out highlightNumbers);
                return highlightNumbers;
            }
            set
            {
                AppSettings.StoreSetting("HighlightNumbers", value);
                RaisePropertyChanged("HighlightNumbers");
            }
        }

        public bool HighlightRowColumn
        {
            get
            {
                bool highlightRowColumn;
                AppSettings.TryGetSetting("HighlightRowColumn", out highlightRowColumn);
                return highlightRowColumn;
            }
            set
            {
                AppSettings.StoreSetting("HighlightRowColumn", value);
                RaisePropertyChanged("HighlightRowColumn");
            }
        }

        public bool ShowErrors
        {
            get
            {
                bool showErrors;
                AppSettings.TryGetSetting("ShowErrors", out showErrors);
                return showErrors;
            }
            set
            {
                AppSettings.StoreSetting("ShowErrors", value);
                RaisePropertyChanged("ShowErrors");
            }
        }

        public string DifficultyStr
        {
            get
            {
                return Difficulty.ToString();
            }
        }

        public int Difficulty
        {
            get
            {
                int diff;
                AppSettings.TryGetSetting("Difficulty", out diff);
                return diff;
            }
            set
            {
                AppSettings.StoreSetting("Difficulty", value);
                RaisePropertyChanged("Difficulty");
                RaisePropertyChanged("DifficultyStr");
            }
        }
    }
}
