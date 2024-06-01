using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Sudoku.View
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            DataContext = this;
        }

        public String Version
        {
            get
            {
                return App.GetVersion();
            }
        }

        private void ContactMe(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "mic11feedback@gmail.com";
            emailComposeTask.Body = "Hello, MiC11\n";
            emailComposeTask.Subject = "Feedback for Minesweeper " + App.GetVersion();
            emailComposeTask.Show();
        }

        private void RateThisApp(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void MyOtherApps(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceSearchTask searchTask = new MarketplaceSearchTask();
            searchTask.SearchTerms = "MiC11";
            searchTask.Show();
        }
    }
}