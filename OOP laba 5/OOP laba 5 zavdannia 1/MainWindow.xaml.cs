using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HorseRaceSimulator
{
    public partial class MainWindow : Window
    {
        private BettingViewModel vm = new BettingViewModel();
        private List<Horse> Horses = new List<Horse>();
        private Barrier barrier;
        private CancellationTokenSource raceTokenSource;
        private const int FinishLine = 1000;
        private bool raceFinished = false;
        private Random random = new Random();
        private List<Horse> currentParticipants = new List<Horse>();

        public MainWindow()
        {
            InitializeComponent();
            Horses = vm.Horses;
            foreach (var horse in Horses)
            {
                if (horse.Color is SolidColorBrush brush)
                {
                    var color = brush.Color;
                    horse.AnimationFrames = GetHorseAnimation(color);
                }
            }

            DataContext = vm;
        }


        private void SyncSelectedHorsesFromCheckBoxes()
        {
            Horses.First(h => h.Name == "Anton").IsSelected = CheckAnton.IsChecked == true;
            Horses.First(h => h.Name == "Dima").IsSelected = CheckDima.IsChecked == true;
            Horses.First(h => h.Name == "Ivan").IsSelected = CheckIvan.IsChecked == true;
            Horses.First(h => h.Name == "Artem").IsSelected = CheckArtem.IsChecked == true;
            Horses.First(h => h.Name == "Sasha").IsSelected = CheckSasha.IsChecked == true;
            Horses.First(h => h.Name == "Anna").IsSelected = CheckAnna.IsChecked == true;
            Horses.First(h => h.Name == "Oleg").IsSelected = CheckOleg.IsChecked == true;
            Horses.First(h => h.Name == "Max").IsSelected = CheckMax.IsChecked == true;
            Horses.First(h => h.Name == "Vova").IsSelected = CheckVova.IsChecked == true;
            Horses.First(h => h.Name == "Igor").IsSelected = CheckIgor.IsChecked == true;
        }
        private void NextHorse_Click(object sender, RoutedEventArgs e)
        {
            SyncSelectedHorsesFromCheckBoxes();
            vm.NextHorse();
        }

        private void PreviousHorse_Click(object sender, RoutedEventArgs e)
        {
            SyncSelectedHorsesFromCheckBoxes();
            vm.PreviousHorse();
        }
        private void BetAmountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Забороняємо вводити нечислові символи
            e.Handled = !int.TryParse(e.Text, out _);
        }
        private void Bet_Click(object sender, RoutedEventArgs e)
        {
            SyncSelectedHorsesFromCheckBoxes();

            var currentParticipants = Horses.Where(h => h.IsSelected).ToList();

            var selectedHorse = currentParticipants.FirstOrDefault(h => h.Name == vm.SelectedHorseText);
            if (selectedHorse == null)
            {
                MessageBox.Show("Selected horse is not participating in the race!");
                return;
            }

            if (vm.Balance < vm.BetAmount)
            {
                MessageBox.Show("Not enough money.");
                return;
            }

            // Не скидати гроші з усіх коней!
            // Просто додати ставку до вибраного коня:
            selectedHorse.Money += vm.BetAmount;

            vm.Balance -= vm.BetAmount;
            vm.NotifyBalance();

            dataGrid.Items.Refresh();
        }

        private void StartRace_Click(object sender, RoutedEventArgs e)
        {
            var currentParticipants = Horses.Where(h => h.IsSelected).ToList();

            if (!currentParticipants.Any())
            {
                MessageBox.Show("No horses selected for the race!");
                return;
            }

            foreach (var horse in currentParticipants)
            {
                horse.Reset();
                horse.Speed = random.Next(5, 10);
            }

            StartRace(currentParticipants);
        }



        private List<Horse> GetSelectedHorses()
        {
            var selectedNames = new List<string>();
            if (CheckAnton.IsChecked == true) selectedNames.Add("Anton");
            if (CheckDima.IsChecked == true) selectedNames.Add("Dima");
            if (CheckIvan.IsChecked == true) selectedNames.Add("Ivan");
            if (CheckArtem.IsChecked == true) selectedNames.Add("Artem");
            if (CheckSasha.IsChecked == true) selectedNames.Add("Sasha");
            if (CheckAnna.IsChecked == true) selectedNames.Add("Anna");
            if (CheckOleg.IsChecked == true) selectedNames.Add("Oleg");
            if (CheckMax.IsChecked == true) selectedNames.Add("Max");
            if (CheckVova.IsChecked == true) selectedNames.Add("Vova");
            if (CheckIgor.IsChecked == true) selectedNames.Add("Igor");

            return Horses.Where(h => selectedNames.Contains(h.Name)).ToList();
        }
        private void StartRace(List<Horse> selectedHorses)
        {
            raceFinished = false;
            raceTokenSource = new CancellationTokenSource();
            barrier = new Barrier(selectedHorses.Count, _ => Dispatcher.Invoke(DrawHorses));
            currentParticipants = selectedHorses;
            foreach (var horse in selectedHorses)
            {
                horse.StartMoving(
                    barrier,
                    FinishLine,
                    () => Dispatcher.Invoke(DrawHorses),
                    raceTokenSource.Token,
                    OnHorseFinish);
            }
        }


        private void OnHorseFinish(Horse winner)
        {
            if (raceFinished) return;
            raceFinished = true;
            raceTokenSource.Cancel();

            Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"🏁 {winner.Name} wins!\nTime: {winner.Time.TotalSeconds:F2} sec", "Race Result");

                // Перевіряємо, чи користувач ставив на переможця
                if (winner.Money > 0)  // якщо ставка > 0 для переможця
                {
                    int payout = (int)(winner.Money * winner.Coefficient);
                    vm.Balance += payout;
                    MessageBox.Show($"You won on {winner.Name}! +{payout}$");
                }
                else
                {
                    // Якщо ставка на переможця відсутня, показуємо що програли (або можна порахувати сумарно програші)
                    MessageBox.Show($"You lost! No winning bet on {winner.Name}");
                }

                // Оновлюємо коефіцієнти лише для коней, що брали участь у гонці
                UpdateHorseCoefficients(currentParticipants);
                foreach (var horse in Horses)
                    horse.Money = 0;
                dataGrid.Items.Refresh();
                vm.NotifyBalance();
            });
        }


        private void UpdateHorseCoefficients(List<Horse> participatingHorses)
        {
            var sorted = participatingHorses.OrderBy(h => h.Time).ToList();
            int n = participatingHorses.Count;
            for (int i = 0; i < n; i++)
            {
                sorted[i].Coefficient = 1.5 + (n - i - 1) * 0.5;
            }
        }

        private void DrawHorses()
        {
            for (int i = canvas.Children.Count - 1; i >= 1; i--)
                canvas.Children.RemoveAt(i);

            var ranked = Horses.OrderByDescending(h => h.TrackX).ToList();
            for (int i = 0; i < ranked.Count; i++)
                ranked[i].Position = i + 1;

            var selectedHorses = GetSelectedHorses();
            for (int i = 0; i < selectedHorses.Count; i++)
            {
                var horse = selectedHorses[i];

                // TextBlock для імені (буде з'являтись при наведенні)
                var nameText = new TextBlock
                {
                    Text = horse.Name,
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Visibility = Visibility.Hidden,
                    Background = Brushes.White,
                    Padding = new Thickness(2),
                    FontSize = 12
                };

                // Контейнер з подіями миші
                var container = new Border
                {
                    Width = 80,
                    Height = 80,
                    Background = Brushes.Transparent,
                    Child = new Image
                    {
                        Source = horse.CurrentFrame,
                        Width = 80,
                        Height = 80,
                        Stretch = Stretch.Fill
                    }
                };

                // Події наведення
                container.MouseEnter += (s, e) => nameText.Visibility = Visibility.Visible;
                container.MouseLeave += (s, e) => nameText.Visibility = Visibility.Hidden;

                // Розміщення
                double x = horse.TrackX;
                double y = 130 + i * 50;

                Canvas.SetLeft(container, x);
                Canvas.SetTop(container, y);
                canvas.Children.Add(container);

                // Розміщення підпису трохи правіше або вище
                Canvas.SetLeft(nameText, x + 85); // можна x + 0, якщо хочеш над головою
                Canvas.SetTop(nameText, y + 25);
                canvas.Children.Add(nameText);
            }

            dataGrid.Items.Refresh();
        }




        public List<ImageSource> GetHorseAnimation(Color color)
        {
            var bitmaps = ReadImageList(@"C:\Users\Laplace\Desktop\OOP-4\OOP laba 5\OOP laba 5 zavdannia 1\Images\Horses\");
            var masks = ReadImageList(@"C:\Users\Laplace\Desktop\OOP-4\OOP laba 5\OOP laba 5 zavdannia 1\Images\HorsesMask\");
            return bitmaps.Select((img, i) => GetImageWithColor(img, masks[i], color)).ToList();
        }

        private List<BitmapImage> ReadImageList(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.png")
                            .OrderBy(f => f)
                            .Select(f => new BitmapImage(new Uri(f, UriKind.Absolute)))
                            .ToList();
        }

        private ImageSource GetImageWithColor(BitmapImage image, BitmapImage mask, Color tintColor)
        {
            int width = image.PixelWidth, height = image.PixelHeight;
            var imageBmp = new WriteableBitmap(image);
            var maskBmp = new WriteableBitmap(mask);
            var output = new WriteableBitmap(width, height, image.DpiX, image.DpiY, PixelFormats.Bgra32, null);

            int stride = width * 4;
            byte[] imagePixels = new byte[height * stride];
            byte[] maskPixels = new byte[height * stride];
            byte[] resultPixels = new byte[height * stride];

            imageBmp.CopyPixels(imagePixels, stride, 0);
            maskBmp.CopyPixels(maskPixels, stride, 0);

            for (int i = 0; i < imagePixels.Length; i += 4)
            {
                double alphaFactor = maskPixels[i + 3] / 255.0;
                resultPixels[i] = (byte)(imagePixels[i] * (1 - alphaFactor) + tintColor.B * alphaFactor);
                resultPixels[i + 1] = (byte)(imagePixels[i + 1] * (1 - alphaFactor) + tintColor.G * alphaFactor);
                resultPixels[i + 2] = (byte)(imagePixels[i + 2] * (1 - alphaFactor) + tintColor.R * alphaFactor);
                resultPixels[i + 3] = imagePixels[i + 3];
            }

            output.WritePixels(new Int32Rect(0, 0, width, height), resultPixels, stride, 0);
            return output;
        }
    }



    public class BettingViewModel : INotifyPropertyChanged
    {
        public int Balance { get; set; } = 250;
        public int BetAmount { get; set; } = 20;
        private static Random random = new Random();
        public List<Horse> Horses { get; set; } = new List<Horse>()
        {
            new Horse("Anton", RandomBrush(), random.Next(5, 10)),
            new Horse("Dima", RandomBrush(), random.Next(5, 10)),
            new Horse("Ivan", RandomBrush(), random.Next(5, 10)),
            new Horse("Artem", RandomBrush(), random.Next(5, 10)),
            new Horse("Sasha", RandomBrush(), random.Next(5, 10)),
            new Horse("Anna", RandomBrush(), random.Next(5, 10)),
            new Horse("Oleg", RandomBrush(), random.Next(5, 10)),
            new Horse("Max", RandomBrush(), random.Next(5, 10)),
            new Horse("Vova", RandomBrush(), random.Next(5, 10)),
            new Horse("Igor", RandomBrush(), random.Next(5, 10)),
        };

        private int horseIndex = 0;

        public string BetAmountText => $"{BetAmount}$";
        public string BalanceText => $"Balance: {Balance}$";
        public string SelectedHorseText => Horses[horseIndex].Name;

        public void NextHorse()
        {
            var participants = Horses.Where(h => h.IsSelected).ToList();
            if (!participants.Any()) return;

            int index = participants.FindIndex(h => h.Name == SelectedHorseText);
            index = (index + 1) % participants.Count;

            SetHorse(Horses.IndexOf(participants[index]));
        }
        public void PreviousHorse()
        {
            var participants = Horses.Where(h => h.IsSelected).ToList();
            if (!participants.Any()) return;

            int index = participants.FindIndex(h => h.Name == SelectedHorseText);
            index = (index - 1 + participants.Count) % participants.Count;

            SetHorse(Horses.IndexOf(participants[index]));
        }

        private void SetHorse(int index) { horseIndex = index; OnPropertyChanged(nameof(SelectedHorseText)); }

        public void NotifyBalance()
        {
            OnPropertyChanged(nameof(BalanceText));
        }

        private static SolidColorBrush RandomBrush()
        {
            return new SolidColorBrush(Color.FromArgb(255,
                (byte)random.Next(0, 255),
                (byte)random.Next(0, 255),
                (byte)random.Next(0, 255)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}