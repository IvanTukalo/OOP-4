using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HorseRaceSimulator
{
    public class Horse 
    {
        public string Name { get; private set; }
        public SolidColorBrush Color { get; private set; }
        public TimeSpan Time { get; private set; }
        public int TrackX { get; private set; } = 0;
        public int Position { get; set; } = 0;
        public int Speed { get; set; }
        public double Coefficient { get; set; } = 2.0;
        public bool IsSelected { get; set; } = true;
        public int Money { get; set; } = 0;
        public List<ImageSource> AnimationFrames { get; set; } = new List<ImageSource>();
        public int CurrentFrameIndex { get; set; } = 0;
        public ImageSource CurrentFrame => AnimationFrames.Count > 0 ? AnimationFrames[CurrentFrameIndex] : null;

        private Stopwatch stopwatch = new Stopwatch();
        private static readonly Random random = new Random();
        private static readonly object randLock = new object();

        public Horse(string name, SolidColorBrush color, int speed)
        {
            Name = name;
            Color = color;
            Speed = speed;
        }

        public void StartMoving(Barrier barrier, int finishX, Action redraw, CancellationToken token, Action<Horse> onFinish)
        {
            stopwatch.Start();

            Task.Run(async () =>
            {
                while (TrackX < finishX && !token.IsCancellationRequested)
                {
                    double modifier;
                    lock (randLock)
                    {
                        modifier = 1 + random.NextDouble();
                    }

                    int adjustedSpeed = (int)(Speed * modifier);
                    TrackX += adjustedSpeed;

                    try
                    {
                        CurrentFrameIndex = (CurrentFrameIndex + 1) % AnimationFrames.Count;
                        redraw?.Invoke();
                        await Task.Delay(100);
                        barrier.SignalAndWait(token);
                    }
                    catch (OperationCanceledException) { break; }
                }

                if (!token.IsCancellationRequested)
                {
                    stopwatch.Stop();
                    Time = stopwatch.Elapsed;
                    onFinish?.Invoke(this);
                }
            }, token);
        }
        public void Reset()
        {
            TrackX = 0;
            Position = 0;
            Time = TimeSpan.Zero;
            CurrentFrameIndex = 0;
        }
    }
}