// ViewModels.cs
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VoronoiDiagram
{
    // Клас для представлення точки з можливістю відслідковувати зміни (важливо для прив'язки)
    public class PointViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        public double X { get; set; }
        public double Y { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        // Колір, унікальний для кожної точки
        public Color Color { get; } = Color.FromRgb((byte)new Random().Next(50, 230), (byte)new Random(Guid.NewGuid().GetHashCode()).Next(50, 230), (byte)new Random(Guid.NewGuid().GetHashCode()).Next(50, 230));

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Конвертер для зміни кольору виділеної точки
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Red : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}