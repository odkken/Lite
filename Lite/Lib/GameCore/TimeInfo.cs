using System.Diagnostics;

namespace Lite.Lib.GameCore
{
    internal class TimeInfo : ITimeInfo
    {
        public float CurrentDt { get; set; }
        public float CurrentTime => (float) _watch.Elapsed.TotalSeconds;
        private readonly Stopwatch _watch = new Stopwatch();
        private double _previousTick;

        public TimeInfo()
        {
            _watch.Start();
            _previousTick = _watch.Elapsed.TotalSeconds;
        }

        public void Tick()
        {
            var currentTick = _watch.Elapsed.TotalSeconds;
            CurrentDt = (float)(currentTick - _previousTick);
            _previousTick = currentTick;
        }
    }
}