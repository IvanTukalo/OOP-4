using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ButtonGenerator
{
    public partial class MainWindow : Window
    {
        private const double AvoidRadius = 80; // Радіус дії "відштовхування"
        private const double MoveStep = 15;    // На скільки переміщується кнопка

        public MainWindow()
        {
            InitializeComponent();

            // Підписуємося на подію SizeChanged
            this.SizeChanged += MainWindow_SizeChanged;

            // Центруємо вміст при старті
            CenterContent();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterContent();
        }

        private void CenterContent()
        {
            if (MainCanvas.ActualWidth <= 0 || MainCanvas.ActualHeight <= 0) return;

            double textWidth = QuestionText.ActualWidth;
            double buttonWidth = YesButton.ActualWidth + NoButton.ActualWidth + 20;

            double centerX = (MainCanvas.ActualWidth - Math.Max(textWidth, buttonWidth)) / 2;
            double centerY = (MainCanvas.ActualHeight - 150) / 2;

            Canvas.SetLeft(QuestionText, centerX);
            Canvas.SetTop(QuestionText, centerY);

            Canvas.SetLeft(YesButton, centerX);
            Canvas.SetTop(YesButton, centerY + 60);

            Canvas.SetLeft(NoButton, centerX + YesButton.Width + 20);
            Canvas.SetTop(NoButton, centerY + 60);
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Молодець.");
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(MainCanvas);

            double btnX = Canvas.GetLeft(NoButton);
            double btnY = Canvas.GetTop(NoButton);
            double btnCenterX = btnX + NoButton.Width / 2;
            double btnCenterY = btnY + NoButton.Height / 2;

            double dx = btnCenterX - mousePos.X;
            double dy = btnCenterY - mousePos.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance < AvoidRadius)
            {
                double nx = dx / distance;
                double ny = dy / distance;

                double newX = btnX + nx * MoveStep;
                double newY = btnY + ny * MoveStep;

                newX = Math.Max(0, Math.Min(MainCanvas.ActualWidth - NoButton.Width, newX));
                newY = Math.Max(0, Math.Min(MainCanvas.ActualHeight - NoButton.Height, newY));

                Canvas.SetLeft(NoButton, newX);
                Canvas.SetTop(NoButton, newY);
            }
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Це сумно.");
        }
    }
}