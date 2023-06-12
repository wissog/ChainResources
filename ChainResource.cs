using ChainResources.Interfaces;

namespace ChainResources
{
    public class ChainResource<T> : IChainResource<T>
    {
        private readonly LinkedList<IStorage<T>> _storageChain = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private bool _writingToExpiredNeeded = true;
        internal int WriteCounter = 0;
        internal int ReadCounter = 0;

        public ChainResource(List<IStorage<T>> storagesList) 
        {
            if (storagesList != null && storagesList.Count > 0)
            {
                foreach (var storage in storagesList)
                {
                    _storageChain.AddLast(storage);
                }
            }
        }

        public async Task<T> GetValue()
        {
            ReadCounter++;

            T? value = default;

            if (!IsEmpty)
            {
                // go to the first not expired storage

                var storageNode = _storageChain.First;

                while (storageNode?.Value != null)
                {
                    value = await storageNode.Value.Read();

                    if (value == null || storageNode.Value.IsExpired())
                    {
                        storageNode = storageNode.Next;
                        _writingToExpiredNeeded = true;
                    }
                    else
                    {
                        // lock here to avoid multiple writing of the same data
                        await _semaphore.WaitAsync();

                        try
                        {
                            if (storageNode != _storageChain.First && _writingToExpiredNeeded)
                            {
                                var results = await WriteToPreviousExpiredStorages(storageNode, value);
                                _writingToExpiredNeeded = false;
                            }
                        }
                        catch (AggregateException agEx)
                        {
                            agEx.Flatten().Handle(ex =>
                            {
                                if (ex is IOException ioEx)
                                {
                                    throw new Exception("Error writing to file: " + ioEx.Message);
                                }
                                else
                                {
                                    throw ex;
                                }
                            });
                        }
                        finally
                        {
                            _semaphore.Release();
                        }

                        break;
                    }
                }
            }

            return value;
        }

        public bool IsEmpty => _storageChain.Count == 0;

        private async Task<bool[]> WriteToPreviousExpiredStorages(LinkedListNode<IStorage<T>>? storageNode, T value)
        {
            WriteCounter++;

            var writeTaskList = new List<Task<bool>>();

            do
            {
                storageNode = storageNode?.Previous;

                if (storageNode?.Value is IWriteableStorage<T> wrStorage && storageNode.Value.IsExpired())
                {
                    writeTaskList.Add(wrStorage.Write(value));
                }
            }
            while (storageNode != null);

            var results = await Task.WhenAll(writeTaskList);
            return results;
        }
    }
}
