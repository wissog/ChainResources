namespace ChainResources.Interfaces
{
    public interface IWriteableStorage<T> : IStorage<T>
    {
        public Task<bool> Write(T value);
    }
}
