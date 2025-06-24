using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
namespace HorseRaceSimulator
{
    public class Horse : INotifyPropertyChanged
    {
        // Статический генератор случайных чисел для всех экземпляров класса
        private static readonly Random Rng = new Random();

        // Основные свойства коня
        public string Name { get; private set; }
        public SolidColorBrush ColorBrush { get; private set; }
        public List<ImageSource> AnimationFrames { get; private set; }

        // Поля для симуляции
        private double _positionX;
        private TimeSpan _raceTime;
        private double _coefficient;
        private double _betAmount;
        private int _currentFrameIndex = 0;
        private readonly double _baseSpeed;
        private double _currentAcceleration = 0;

        // Свойства с уведомлением об изменении для привязки к DataGrid
        public double PositionX
        {
            get => _positionX;
            set { _positionX = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayPosition)); }
        }

        // Для красивого отображения в таблице
        public int DisplayPosition => (int)_positionX;

        public TimeSpan RaceTime
        {
            get => _raceTime;
            set { _raceTime = value; OnPropertyChanged(); }
        }

        public double Coefficient
        {
            get => _coefficient;
            set { _coefficient = value; OnPropertyChanged(); }
        }

        public double BetAmount
        {
            get => _betAmount;
            set { _betAmount = value; OnPropertyChanged(); }
        }

        public Horse(string name, Color color, List<ImageSource> AnimationFrames)
        {
            Name = name;
            ColorBrush = new SolidColorBrush(color);
            this.AnimationFrames = AnimationFrames ?? new List<ImageSource>();

            // Загружаем и раскрашиваем анимации

            // Инициализация случайных параметров
            _baseSpeed = Rng.Next(5, 11); // Базовая скорость
            Coefficient = Math.Round(1.5 + Rng.NextDouble() * 3, 2); // Начальный коэффициент
        }

        // Асинхронный метод для изменения ускорения
        public async Task ChangeAcceleration()
        {
            // Имитация случайного рывка
            _currentAcceleration = _baseSpeed * (0.7 + Rng.NextDouble() * 0.3);
            // Небольшая задержка, чтобы Task не завершался мгновенно
            await Task.Delay(1);
        }

        // Метод для обновления позиции и анимации
        public void Move()
        {
            PositionX += _currentAcceleration;
            _currentFrameIndex = (_currentFrameIndex + 1) % AnimationFrames.Count;
        }

        // Метод для сброса состояния коня перед новой гонкой
        public void Reset()
        {
            PositionX = 0;
            RaceTime = TimeSpan.Zero;
            BetAmount = 0;
            _currentFrameIndex = 0;
        }

        // Метод для отрисовки коня на холсте
        public void Render(DrawingContext dc, double yPosition)
        {
            if (AnimationFrames != null && AnimationFrames.Any())
            {
                var frame = AnimationFrames[_currentFrameIndex];
                // Рисуем коня в его текущей X позиции и на переданной Y-дорожке
                dc.DrawImage(frame, new Rect(PositionX, yPosition, frame.Width, frame.Height));
            }
        }

        #region Код для загрузки и раскраски изображений (из задания)

        #endregion

        #region Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}