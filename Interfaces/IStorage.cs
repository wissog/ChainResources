namespace ChainResources.Interfaces
{
    public interface IStorage<T>
    {
        public string Name { get; }

        public bool IsExpired();

        public Task<T> Read();
    }
}
