using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sudoku.ViewModel;

namespace Sudoku.View
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private SettingsViewModel ViewModel
        {
            get
            {
                return this.DataContext as SettingsViewModel;
            }
        }

        private void HighlightNumbers(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.HighlightNumbers = !ViewModel.HighlightNumbers;
        }

        private void HighlightRowColumn(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.HighlightRowColumn = !ViewModel.HighlightRowColumn;
        }

        private void ShowErrors(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.ShowErrors = !ViewModel.ShowErrors;
        }
    }
}