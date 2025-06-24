using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HorseRaceSimulator
{
    public partial class MainWindow : Window
    {
        // Коллекция для хранения данных о конях. ObservableCollection сама уведомляет UI об изменениях.
        public ObservableCollection<Horse> Horses { get; set; }

        // Таймер для отрисовки (лучше для WPF, чем другие таймеры)
        private readonly DispatcherTimer _renderTimer = new DispatcherTimer();
        // Таймер для отсчета времени гонки
        private readonly Stopwatch _raceStopwatch = new Stopwatch();

        private bool _isRaceInProgress = false;
        private double _balance = 250;
        private const int FinishLineX = 1050; // Координата финишной черты

        private readonly ImageSource _trackBackground;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Устанавливаем контекст данных для привязок в XAML

            // Загружаем фон
            _trackBackground = new BitmapImage(new Uri("C:\\Users\\Laplace\\Desktop\\OOP-4\\OOP laba 5\\OOP laba 5 zavdannia 1\\Images\\Background\\Track.png"));

            // Настраиваем таймер отрисовки
            _renderTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            _renderTimer.Tick += RenderTimer_Tick;

            // Инициализация начального состояния
            HorseCountComboBox.SelectedIndex = 2; // По умолчанию 4 коня (индекс 2)
            SetupRace();
            UpdateBalanceUI();
        }

        // Подготовка к гонке: создание коней, сброс состояния

        public List<ImageSource> GetHorseAnimation(Color color)
        {
            const int count = 12;
            // Вызываем ReadImageList с правильными путями, которые начинаются с pack://
            var bitmapImageList = ReadImageList("pack://application:,,,/Images/Horses/", "WithOutBorder_", ".png", count);
            var maskImageList = ReadImageList("pack://application:,,,/Images/HorsesMask/", "mask_", ".png", count);
            return bitmapImageList.Select((item, index) => GetImageWithColor(item, maskImageList[index], color)).ToList();
        }

        private List<BitmapImage> ReadImageList(string path, string name, string format, int count)
        {
            List<BitmapImage> list = new List<BitmapImage>();
            for (int i = 0; i < count; i++)
            {
                // Простое объединение строк для формирования полного и корректного Pack URI
                // Например: "pack://application:,,,/Images/Horses/WithOutBorder_0000.png"
                var fullUri = path + name + $"{i:0000}" + format;

                // Создаем Uri из корректной строки
                var uri = new Uri(fullUri, UriKind.Absolute);

                // Теперь BitmapImage сможет найти и загрузить ресурс
                var img = new BitmapImage(uri);
                list.Add(img);
            }
            return list;
        }

        private ImageSource GetImageWithColor(BitmapImage image, BitmapImage mask, Color color)
        {
            // Этот код для WriteableBitmapEx у тебя был правильным, его не трогаем
            WriteableBitmap imageBmp = new WriteableBitmap(image);
            WriteableBitmap maskBmp = new WriteableBitmap(mask);
            WriteableBitmap outputBmp = BitmapFactory.New(image.PixelWidth, image.PixelHeight);

            outputBmp.ForEach((x, y, c) =>
            {
                var maskPixel = maskBmp.GetPixel(x, y);
                if (maskPixel.A > 0)
                {
                    return MultiplyColors(imageBmp.GetPixel(x, y), color, maskPixel.A);
                }
                return imageBmp.GetPixel(x, y);
            });

            return outputBmp;
        }

        private Color MultiplyColors(Color color1, Color color2, byte alpha)
        {
            var amount = alpha / 255.0;
            byte r = (byte)(color2.R * amount + color1.R * (1 - amount));
            byte g = (byte)(color2.G * amount + color1.G * (1 - amount));
            byte b = (byte)(color2.B * amount + color1.B * (1 - amount));
            return Color.FromArgb(color1.A, r, g, b);
        }

        private void SetupRace()
        {
            _isRaceInProgress = false;
            _raceStopwatch.Reset();

            int horseCount = int.Parse((HorseCountComboBox.SelectedItem as ComboBoxItem).Content.ToString());

            // Данные для создания коней
            var horseData = new[]
            {
                new { Name = "Счастливчик", Color = Colors.Red },
                new { Name = "Рейнджер", Color = Colors.Green },
                new { Name = "Ива", Color = Colors.Blue },
                new { Name = "Такер", Color = Colors.Yellow },
                new { Name = "Буря", Color = Colors.Purple },
                new { Name = "Призрак", Color = Colors.Orange }
            };

            Horses = new ObservableCollection<Horse>();
            for (int i = 0; i < horseCount; i++)
            {
                Horses.Add(new Horse(horseData[i].Name, horseData[i].Color, GetHorseAnimation(horseData[i].Color)));
            }

            // Обновляем источники данных для UI
            ResultsGrid.ItemsSource = Horses;
            HorsesComboBox.ItemsSource = Horses;
            HorsesComboBox.SelectedIndex = 0;

            // Отрисовываем начальное состояние
            RenderFrame();

            // Включаем контролы
            SetControlsEnabled(true);
        }

        // Обработчик нажатия на кнопку "СТАРТ"
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRaceInProgress) return;

            // Сброс состояния перед стартом
            foreach (var horse in Horses)
            {
                horse.Reset();
            }

            _isRaceInProgress = true;
            SetControlsEnabled(false);

            _raceStopwatch.Start();
            _renderTimer.Start();

            // Запускаем основной цикл гонки асинхронно
            await RaceLoop();
        }

        // Основной цикл симуляции
        private async Task RaceLoop()
        {
            while (_isRaceInProgress)
            {
                // 1. Создаем список задач для асинхронного изменения ускорения каждого коня
                List<Task> tasks = new List<Task>();
                foreach (var horse in Horses)
                {
                    tasks.Add(horse.ChangeAcceleration());
                }
                // 2. Ждем, пока все кони определят свое ускорение на этот кадр
                await Task.WhenAll(tasks);

                // 3. Обновляем время, позицию и анимацию для каждого коня
                var elapsed = _raceStopwatch.Elapsed;
                foreach (var horse in Horses)
                {
                    horse.RaceTime = elapsed;
                    horse.Move();
                }

                // 4. Проверяем, есть ли победитель
                var winner = Horses.FirstOrDefault(h => h.PositionX >= FinishLineX);
                if (winner != null)
                {
                    EndRace(winner);
                    break; // Выходим из цикла
                }

                // Небольшая задержка, чтобы не перегружать CPU и дать UI время на отклик
                await Task.Delay(16);
            }
        }

        // Метод, вызываемый каждый тик таймера для отрисовки
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            RenderFrame();
        }

        // Метод для отрисовки одного кадра
        private void RenderFrame()
        {
           // var renderBitmap = new RenderTargetBitmap((int)RaceTrackImage.ActualWidth, (int)RaceTrackImage.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            var drawingVisual = new DrawingVisual();

            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                // Рисуем фон
           //     dc.DrawImage(_trackBackground, new Rect(0, 0, renderBitmap.Width, renderBitmap.Height));

                // Рисуем каждого коня
                if (Horses != null)
                {
                    // Распределяем коней по дорожкам
                    double trackHeight = 80;
                    for (int i = 0; i < Horses.Count; i++)
                    {
                        double yPos = 100 + i * trackHeight;
                        Horses[i].Render(dc, yPos);
                    }
                }
            }

            //renderBitmap.Render(drawingVisual);
            //RaceTrackImage.Source = renderBitmap;

            // Динамическая сортировка таблицы
            var sortedHorses = new ObservableCollection<Horse>(Horses.OrderByDescending(h => h.PositionX));
            ResultsGrid.ItemsSource = sortedHorses;
        }

        // Завершение гонки
        private void EndRace(Horse winner)
        {
            _isRaceInProgress = false;
            _renderTimer.Stop();
            _raceStopwatch.Stop();

            // Расчет выигрыша
            var betOnHorse = Horses.FirstOrDefault(h => h.BetAmount > 0);
            string resultMessage = $"Победил {winner.Name}!";
            if (betOnHorse != null && betOnHorse == winner)
            {
                double winnings = betOnHorse.BetAmount * betOnHorse.Coefficient;
                _balance += winnings;
                resultMessage += $"\nПоздравляем! Вы выиграли {winnings:C}";
            }
            else if (betOnHorse != null)
            {
                resultMessage += $"\nК сожалению, ваша ставка проиграла.";
            }

            MessageBox.Show(resultMessage, "Гонка завершена");

            // Обновление коэффициентов
            UpdateCoefficients();
            UpdateBalanceUI();

            // Включаем контролы для следующей гонки
            SetControlsEnabled(true);
        }

        // Обновление коэффициентов после гонки
        private void UpdateCoefficients()
        {
            var sortedHorses = Horses.OrderByDescending(h => h.PositionX).ToList();
            for (int i = 0; i < sortedHorses.Count; i++)
            {
                var horse = sortedHorses[i];
                // Уменьшаем коэффициент победителю, увеличиваем проигравшим
                if (i == 0) // Победитель
                {
                    horse.Coefficient = Math.Max(1.1, horse.Coefficient - 0.5);
                }
                else // Проигравшие
                {
                    horse.Coefficient += 0.1 * i;
                }
                horse.Coefficient = Math.Round(horse.Coefficient, 2);
            }
        }

        private void BetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(BetAmountTextBox.Text, out double betAmount) || betAmount <= 0)
            {
                MessageBox.Show("Сумма ставки должна быть положительным числом.", "Ошибка");
                return;
            }

            if (betAmount > _balance)
            {
                MessageBox.Show("Недостаточно средств на балансе.", "Ошибка");
                return;
            }

            var selectedHorse = HorsesComboBox.SelectedItem as Horse;
            if (selectedHorse == null)
            {
                MessageBox.Show("Пожалуйста, выберите коня для ставки.", "Ошибка");
                return;
            }

            // Сброс предыдущих ставок
            foreach (var h in Horses) h.BetAmount = 0;

            // Установка новой ставки
            selectedHorse.BetAmount = betAmount;
            _balance -= betAmount;

            UpdateBalanceUI();
            MessageBox.Show($"Вы поставили {betAmount:C} на коня {selectedHorse.Name}.", "Ставка принята");
        }

        private void HorseCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded && !_isRaceInProgress) // Проверяем, что окно загружено и гонка не идет
            {
                SetupRace();
            }
        }

        private void UpdateBalanceUI()
        {
            BalanceText.Text = $"{_balance:C}"; // "C" - формат валюты
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            StartButton.IsEnabled = isEnabled;
            BetButton.IsEnabled = isEnabled;
            HorseCountComboBox.IsEnabled = isEnabled;
            BetAmountTextBox.IsEnabled = isEnabled;
            HorsesComboBox.IsEnabled = isEnabled;
        }
    }
}