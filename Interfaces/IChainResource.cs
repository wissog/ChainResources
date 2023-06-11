namespace ChainResources.Interfaces
{
    internal interface IChainResource<T>
    {
        Task<T> GetValue();
    }
}
