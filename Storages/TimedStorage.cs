using System.Timers;
using Timer = System.Timers.Timer;

namespace ChainResources.Storages
{
    internal class TimedStorage : IDisposable
    {
        protected Timer _timer;

        public bool Expired { get; set; }

        public TimedStorage(int hours)
        {
            int milliseconds = hours * 1000 * 60 * 60;

            _timer = new Timer(milliseconds);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Expired = true;
        }

        public virtual void Dispose()
        {
            _timer.Dispose();
        }
    }
}
