using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OOP_laba_1_zavdanya_5
{
    public partial class MainWindow : Window
    {
        List<Action> superActions = new List<Action>();
        private bool isBlue = true;
        private double opacityStep = 0.1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoTransparency()
        {
            this.Opacity = this.Opacity > 0.9 ? 0.5 : this.Opacity + opacityStep;
        }

        private void DoChangeBackground()
        {
            MainBorder.Background = isBlue ? Brushes.LightGoldenrodYellow : Brushes.LightSkyBlue;
            isBlue = !isBlue;
        }


        private void DoHello()
        {
            MessageBox.Show("Hello from classic button!\nУ вас спина біла", "Помідорлення");
        }

        private void btnTransparency_Click(object sender, RoutedEventArgs e) => DoTransparency();
        private void btnBackground_Click(object sender, RoutedEventArgs e) => DoChangeBackground();
        private void btnHello_Click(object sender, RoutedEventArgs e) => DoHello();

        private void chkTransparency_Checked(object sender, RoutedEventArgs e) => AddAction(DoTransparency);
        private void chkTransparency_Unchecked(object sender, RoutedEventArgs e) => RemoveAction(DoTransparency);

        private void chkBackground_Checked(object sender, RoutedEventArgs e) => AddAction(DoChangeBackground);
        private void chkBackground_Unchecked(object sender, RoutedEventArgs e) => RemoveAction(DoChangeBackground);

        private void chkHello_Checked(object sender, RoutedEventArgs e) => AddAction(DoHello);
        private void chkHello_Unchecked(object sender, RoutedEventArgs e) => RemoveAction(DoHello);

        private void AddAction(Action action)
        {
            if (!superActions.Contains(action))
                superActions.Add(action);
        }

        private void RemoveAction(Action action)
        {
            if (superActions.Contains(action))
                superActions.Remove(action);
        }

        private void btnSuper_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Я супернегакнопка,\nі цього мене не позбавиш!");
            foreach (var action in superActions)
                action.Invoke();
        }

        // Заголовок — перетягування вікна
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
