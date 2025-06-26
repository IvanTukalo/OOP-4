// MainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VoronoiDiagram
{
    public partial class MainWindow : Window
    {
        // Використовуємо ObservableCollection для автоматичного оновлення UI
        public ObservableCollection<PointViewModel> Points { get; } = new ObservableCollection<PointViewModel>();
        private Dictionary<PointViewModel, int> _lastAreas = new Dictionary<PointViewModel, int>();
        private readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            // Встановлюємо контекст даних для прив'язки {Binding Points}
            DataContext = this;
        }

        // Кнопка для запуску побудови
        private void DrawDiagramButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Points.Any())
            {
                MessageBox.Show("Будь ласка, додайте хоча б одну точку на полотно.");
                // Очищення старого зображення, якщо точки видалили
                VoronoiImage.Source = null;
                return;
            }

            bool useParallel = ModeComboBox.SelectedIndex == 1;
            int metricIndex = MetricComboBox.SelectedIndex;

            // Створюємо екземпляр нашого алгоритму і запускаємо його
            var algorithm = new VoronoiAlgorithm(VoronoiImage, Points.ToList(), useParallel, metricIndex);
            var (realTime, cpuTime, areas) = algorithm.Run();

            // Зберігаємо результати і оновлюємо UI
            _lastAreas = areas;
            RealTimeTextBlock.Text = $"{realTime.TotalMilliseconds:F2} ms";
            CpuTimeTextBlock.Text = $"{cpuTime.TotalMilliseconds:F2} ms";
        }
        // Додавання точки по кліку на полотні
        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition((IInputElement)sender);
            Points.Add(new PointViewModel { X = position.X - 4, Y = position.Y - 4 }); // -4 для центрування
        }

        // Виділення точки (і зняття виділення з інших)
        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && ellipse.DataContext is PointViewModel clickedPoint)
            {
                // Знімаємо виділення з усіх точок
                foreach (var p in Points)
                {
                    p.IsSelected = false;
                }
                // Виділяємо обрану
                clickedPoint.IsSelected = true;

                // Зупиняємо подальше розповсюдження події, щоб не створювалась нова точка
                e.Handled = true;
            }
        }

        // Видалення виділеної точки по натисканню клавіші Delete
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var selectedPoint = Points.FirstOrDefault(p => p.IsSelected);
                if (selectedPoint != null)
                {
                    Points.Remove(selectedPoint);
                }
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(RandomPointsCount.Text, out int count) || count <= 0) return;

            // Важливо! Отримуємо розмір з батьківського елемента Image
            var drawingGrid = (FrameworkElement)VoronoiImage.Parent;
            if (drawingGrid.ActualWidth < 1 || drawingGrid.ActualHeight < 1)
            {
                MessageBox.Show("Розмір області для малювання ще не визначено. Спробуйте змінити розмір вікна і повторити.");
                return;
            }

            Points.Clear();
            for (int i = 0; i < count; i++)
            {
                Points.Add(new PointViewModel
                {
                    X = _random.Next(0, (int)drawingGrid.ActualWidth - 8),
                    Y = _random.Next(0, (int)drawingGrid.ActualHeight - 8)
                });
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Points.Clear();
            VoronoiImage.Source = null;
        }

        private void RemoveSmallestButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_lastAreas.Any())
            {
                MessageBox.Show("Спочатку побудуйте діаграму, щоб розрахувати площі.");
                return;
            }
            if (!double.TryParse(RemovePercentage.Text, out double percentage) || percentage <= 0 || percentage > 100)
            {
                MessageBox.Show("Введіть коректний відсоток (0-100).");
                return;
            }

            var sortedPoints = _lastAreas.OrderBy(kv => kv.Value).ToList();
            int countToRemove = (int)(Points.Count * (percentage / 100.0));

            for (int i = 0; i < countToRemove && i < sortedPoints.Count; i++)
            {
                Points.Remove(sortedPoints[i].Key);
            }

            //Після видалення потрібно перемалювати діаграму
            DrawDiagramButton_Click(null, null);
        }

    }
}