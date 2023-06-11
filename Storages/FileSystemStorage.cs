using ChainResources.Interfaces;

namespace ChainResources.Storages
{
    internal class FileSystemStorage : TimedStorage, IWriteableStorage<ExchangeRateList?>
    {
        private string _folderName;
        private string _fileName;
        private ExchangeRateList? _exchangeRateList = null;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public FileSystemStorage(string folderName, string fileName) : base(4)
        {
            this.Name = "FileSystem";
            _folderName = folderName;
            _fileName = fileName;
        }

        public string Name { get; }

        public bool IsExpired()
        {
            return base.Expired || _exchangeRateList == null;
        }

        private string FileName => Path.Combine(_folderName, _fileName);

        public async Task<ExchangeRateList?> Read()
        {
            try
            {
                // sync reading and writing to the storage
                await _semaphore.WaitAsync();

                if (File.Exists(FileName) && !IsExpired())
                {
                    var json = await File.ReadAllTextAsync(FileName);
                    _exchangeRateList = await Task.Run(() => ExchangeRateList.ConvertFromJson(json));

                    return _exchangeRateList;
                }
            }
            catch (Exception ex)
            {
                // TODO handle reading error
            }
            finally
            {
                _semaphore.Release();
            }

            return null;
        }

        public async Task<bool> Write(ExchangeRateList? exchangeRateList)
        {
            try
            {
                await _semaphore.WaitAsync();
               
                if (exchangeRateList != null && Directory.Exists(_folderName))
                {
                    await File.WriteAllTextAsync(FileName, exchangeRateList.GetAsJson());
                    _timer.Start();
           
                    return true;
                }
            }
            catch (Exception ex)
            {
                // add exception handling here
                throw new Exception("Error writing to file " + FileName, ex);
            }
            finally
            {
                _semaphore.Release();
            }

            return false;
        }
    }
}
