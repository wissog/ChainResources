using ChainResources.Interfaces;

namespace ChainResources.Storages
{
    internal class MemoryStorage<T> : TimedStorage, IWriteableStorage<T?>, IDisposable
    {
        private T? _data;
        private SemaphoreSlim _semaphore = new(1, 1);

        public MemoryStorage() : base(1)
        {
            this.Name = "Memory";
        }

        public string Name { get; }

        public bool IsExpired()
        {
            return base.Expired || _data == null;
        }

        public async Task<T?> Read()
        {
            await _semaphore.WaitAsync();

            var result = IsExpired() ? default : _data;
            
            _semaphore.Release();
            return result;
        }

        public async Task<bool> Write(T? data)
        {
            await _semaphore.WaitAsync();

            var result = false;

            if (_data != null)
            {
                _data = data;
                _timer.Start();
                result = true;
            }

            _semaphore.Release();
            return result;
        }
    }
}
