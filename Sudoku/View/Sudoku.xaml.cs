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
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;

namespace Sudoku.View
{
    public partial class Sudoku : PhoneApplicationPage
    {
        // Constructor
        public Sudoku()
        {
            InitializeComponent();
            //prevent lock screen
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            Numpad.Opacity = 0;
            board.Opacity = 0;
            GenerateBoardView();
        }

        public void ShowBoard(TimeSpan duration)
        {
            List<UIElement> subsquares = board.Children.ToList();

            //preparing easing function
            QuinticEase QuinticOut = new QuinticEase();
            QuinticOut.EasingMode = EasingMode.EaseOut;

            SineEase QuarticInOut = new SineEase();
            QuarticInOut.EasingMode = EasingMode.EaseInOut;
            
            //Opacity animation
            var OpacAnim = new DoubleAnimation();
            OpacAnim.From = 0.0;
            OpacAnim.To = 1.0;
            OpacAnim.Duration = new Duration(duration);
            OpacAnim.EasingFunction = QuarticInOut;

            // create the storyboard
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(OpacAnim);

            Storyboard.SetTarget(OpacAnim, board);
            Storyboard.SetTargetProperty(OpacAnim, new PropertyPath("(UIElement.Opacity)"));

            //moving subsquares animation
            foreach (UIElement el in subsquares)
            {
                if (el is Grid)
                {
                    int x = (int)el.GetValue(Grid.ColumnProperty);
                    int y = (int)el.GetValue(Grid.RowProperty);
                    double mx=0, my=0;

                    if ((x + y) % 2 == 0)
                    {
                        mx = x == 0 ? -50 : 50;
                        my = y == 0 ? -50 : 50;
                        if (x == 1 && y == 1)
                        {
                            mx = 0;
                            my = 0;
                        }
                    }
                    else
                    {
                        if (y == 0) my = 50;
                        else if (y == 2) my = -50;
                        if (x == 0) mx = 50;
                        else if (x == 2) mx = -50;
                    }

                    //anim
                    var MoveXAnim = new DoubleAnimation()
                    {
                        From = mx,
                        To = 0,
                        Duration = duration,
                        EasingFunction = QuinticOut
                    };
                    var MoveYAnim = new DoubleAnimation()
                    {
                        From = my,
                        To = 0,
                        Duration = duration,
                        EasingFunction = QuinticOut
                    };

                    el.RenderTransform = new CompositeTransform();
                    storyboard.Children.Add(MoveXAnim);
                    storyboard.Children.Add(MoveYAnim);
                    Storyboard.SetTarget(MoveXAnim, el);
                    Storyboard.SetTargetProperty(MoveXAnim, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
                    Storyboard.SetTarget(MoveYAnim, el);
                    Storyboard.SetTargetProperty(MoveYAnim, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
                }
            }

            // start the annimation
            storyboard.Begin();
        }

        public void ShowNumpad()
        {
            Numpad.RenderTransform = new CompositeTransform();
            //preparing easing function
            QuinticEase QuinticOut = new QuinticEase();
            QuinticOut.EasingMode = EasingMode.EaseOut;

            QuadraticEase QuarticInOut = new QuadraticEase();
            QuarticInOut.EasingMode = EasingMode.EaseInOut;

            //Opacity animation
            var OpacAnim = new DoubleAnimation();
            OpacAnim.BeginTime = new TimeSpan(0, 0, 0, 0, 400);
            OpacAnim.From = 0.0;
            OpacAnim.To = 1.0;
            OpacAnim.Duration = new Duration(new TimeSpan(0, 0, 0, 1, 500));
            OpacAnim.EasingFunction = QuarticInOut;

            //Move animation
            var MoveAnim = new DoubleAnimation();
            MoveAnim.BeginTime = new TimeSpan(0, 0, 0, 0, 400);
            MoveAnim.From = -30;
            MoveAnim.To = 0;
            MoveAnim.Duration = new Duration(new TimeSpan(0, 0, 0, 1, 500));
            MoveAnim.EasingFunction = QuinticOut;

            // create the storyboard
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(OpacAnim);
            storyboard.Children.Add(MoveAnim);

            Storyboard.SetTarget(OpacAnim, Numpad);
            Storyboard.SetTarget(MoveAnim, Numpad);
            Storyboard.SetTargetProperty(OpacAnim, new PropertyPath("(UIElement.Opacity)"));
            Storyboard.SetTargetProperty(MoveAnim, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            //"(UIElement.RenderTransform).(CompositeTransform.TranslateX)"

            // start the annimation
            storyboard.Begin();
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SudokuState")
            {
                GameState gstate = ViewModel.SudokuState;
                if (gstate == GameState.Finished)
                {
                    SudokuBoardFinished.Begin();
                    UnfocusSudoku();
                }
            }
        }

        private void GenerateBoardView()
        {
            List<List<FieldViewModel>> subsqrs = ViewModel.AllSubsquares();

            foreach (List<FieldViewModel> subsqr in subsqrs)
            {
                int subx = (subsqr.First().Model.X - 1) / 3;
                int suby = (subsqr.First().Model.Y - 1) / 3;
                Grid subgrid = new Grid();
                subgrid.RowDefinitions.Add(new RowDefinition());
                subgrid.RowDefinitions.Add(new RowDefinition());
                subgrid.RowDefinitions.Add(new RowDefinition());
                subgrid.ColumnDefinitions.Add(new ColumnDefinition());
                subgrid.ColumnDefinitions.Add(new ColumnDefinition());
                subgrid.ColumnDefinitions.Add(new ColumnDefinition());
                subgrid.Margin = new Thickness(1);


                Grid.SetRow(subgrid, suby);
                Grid.SetColumn(subgrid, subx);
                board.Children.Add(subgrid);

                foreach (FieldViewModel f in subsqr)
                {
                    TextBlock tbx = new TextBlock();
                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("Number");
                    tbx.SetBinding(TextBlock.TextProperty, binding);
                    tbx.FontSize = 30;
                    tbx.FontWeight = FontWeights.Bold;

                    binding = new Binding();
                    binding.Path = new PropertyPath("ForeColorBrush");
                    tbx.SetBinding(TextBlock.ForegroundProperty, binding);

                    tbx.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    tbx.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    Grid grid = new Grid();

                    binding = new Binding();
                    binding.Path = new PropertyPath("BackColorBrush");
                    grid.SetBinding(Grid.BackgroundProperty, binding);

                    grid.Tap += field_Tap;
                    grid.Children.Add(tbx);

                    Border border = new Border();
                    border.Child = grid;
                    binding = new Binding();
                    binding.Source = f;
                    border.SetBinding(Border.DataContextProperty, binding);
                    border.BorderThickness = new Thickness(1);

                    binding = new Binding();
                    binding.Path = new PropertyPath("MarginColorBrush");
                    border.SetBinding(Border.BorderBrushProperty, binding);

                    int x = (f.Model.X - 1) % 3;
                    int y = (f.Model.Y - 1) % 3;
                    Grid.SetRow(border, y);
                    Grid.SetColumn(border, x);
                    subgrid.Children.Add(border);
                }
            }
        }

        void field_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid field = sender as Grid;
            FieldViewModel fieldVM = (FieldViewModel)field.DataContext;
            ViewModel.FocusField(fieldVM);
        }

        private SudokuViewModel ViewModel
        {
            get
            {
                return this.DataContext as SudokuViewModel;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            board.Opacity = 0;
            Numpad.Opacity = 0;

            if (ViewModel.SudokuState == GameState.GameOn)
            {
                ViewModel.Pause();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ViewModel.SudokuState == GameState.Pause)
            {
                ViewModel.Start();
            }
            ShowBoard(new TimeSpan(0, 0, 0, 1, 500));
            ShowNumpad();

        }

        private void NumberButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button butt = sender as Button;

            if ((butt.Tag as string) == "C")
            {
                ViewModel.SetNumber(null);
                return;
            }

            int number;
            bool parseResult = int.TryParse(butt.Tag as string, out number);

            if (parseResult == true)
            {
                ViewModel.SetNumber(number);
            }
        }

        private void UnfocusSudoku()
        {
            ViewModel.Unfocus();
            this.Focus();
        }


        private void AppBar_AddNumber(object sender, EventArgs e)
        {
            ViewModel.AddRandomNumber();
        }

        private void AppBar_Solve(object sender, EventArgs e)
        {
            ViewModel.Solve();
        }

        private void AppBar_NewGame(object sender, EventArgs e)
        {
            ViewModel.Refresh();
            UnfocusSudoku();
        }

        private void AppBar_Settings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Settings.xaml", UriKind.Relative));
        }

        private void AppBar_About(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/About.xaml", UriKind.Relative));
        }

    }
}