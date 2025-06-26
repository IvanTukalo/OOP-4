// VoronoiAlgorithm.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VoronoiDiagram
{
    // Клас для інкапсуляції всієї логіки побудови діаграми
    public class VoronoiAlgorithm
    {
        private readonly Image _imageControl;
        private readonly List<PointViewModel> _points;
        private readonly bool _useParallel;
        private readonly int _metricIndex;

        public VoronoiAlgorithm(Image imageControl, List<PointViewModel> points, bool useParallel, int metricIndex)
        {
            _imageControl = imageControl;
            _points = points;
            _useParallel = useParallel;
            _metricIndex = metricIndex;
        }

        // Запускає побудову і повертає статистику
        public (TimeSpan RealTime, TimeSpan CpuTime, Dictionary<PointViewModel, int> Areas) Run()
        {
            // НАДІЙНЕ отримання розмірів від батьківського елемента
            int width = (int)((FrameworkElement)_imageControl.Parent).ActualWidth;
            int height = (int)((FrameworkElement)_imageControl.Parent).ActualHeight;

            if (width == 0 || height == 0 || !_points.Any())
            {
                return (TimeSpan.Zero, TimeSpan.Zero, new Dictionary<PointViewModel, int>());
            }

            var bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            var areas = _points.ToDictionary(p => p, _ => 0);

            var process = Process.GetCurrentProcess();
            TimeSpan cpuBefore = process.TotalProcessorTime;
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (_useParallel)
            {
                // Розбиття на області для паралельної обробки
                Parallel.For(0, height, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        ProcessPixel(x, y, width, stride, pixels, areas);
                    }
                });
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        ProcessPixel(x, y, width, stride, pixels, areas);
                    }
                }
            }

            stopwatch.Stop();
            TimeSpan cpuAfter = process.TotalProcessorTime;

            bmp.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            _imageControl.Source = bmp;

            return (stopwatch.Elapsed, cpuAfter - cpuBefore, areas);
        }

        private void ProcessPixel(int x, int y, int width, int stride, byte[] pixels, Dictionary<PointViewModel, int> areas)
        {
            double minDistance = double.MaxValue;
            PointViewModel closestPoint = null;

            foreach (var point in _points)
            {
                double dist = GetDistance(point.X, point.Y, x, y);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestPoint = point;
                }
            }

            if (closestPoint != null)
            {
                int index = y * stride + x * 4;
                pixels[index] = closestPoint.Color.B;
                pixels[index + 1] = closestPoint.Color.G;
                pixels[index + 2] = closestPoint.Color.R;
                pixels[index + 3] = 255;

                // Потокобезпечне оновлення лічильника площі
                lock (areas)
                {
                    areas[closestPoint]++;
                }
            }
        }

        private double GetDistance(double p1x, double p1y, double p2x, double p2y)
        {
            double dx = p1x - p2x;
            double dy = p1y - p2y;

            switch (_metricIndex)
            {
                case 1: // Манхеттенська
                    return Math.Abs(dx) + Math.Abs(dy);
                case 2: // Чебишова
                    return Math.Max(Math.Abs(dx), Math.Abs(dy));
                default: // Евклідова (і випадок 0)
                    return dx * dx + dy * dy;
            }
        }
    }
}