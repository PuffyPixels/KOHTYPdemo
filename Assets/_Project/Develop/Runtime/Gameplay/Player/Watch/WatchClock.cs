using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Watch
{
    public class WatchClock : IDisposable
    {
        private int _lastMinute = -1;

        public event Action<int, int> TimeChanged;

        public void Tick()
        {
            DateTime now = DateTime.Now;

            if (now.Minute == _lastMinute)
                return;

            _lastMinute = now.Minute;

            TimeChanged?.Invoke(now.Hour, now.Minute);
        }

        public void Dispose()
        {
            TimeChanged = null;
        }
    }
}