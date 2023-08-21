using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;

internal class DefaultDataLayerFactory<TValue> : IDataLayerFactory<TValue>
{
    private readonly IAsyncExternalDataAllocator<TValue> dataAllocator;

    public DefaultDataLayerFactory(IAsyncExternalDataAllocator<TValue> dataAllocator)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
    }
    public IAsyncMapInternal<TKey, TValue> Create<TKey>() where TKey : IKey
    {
        return new DefaultAsyncMapInternal<TKey, TValue>(dataAllocator, this);
    }

    public IAsyncMapInternal<TKey, TValue> Create<TKey>(IAsyncExternalDataAllocator<TValue> externalDataAllocator) where TKey : IKey
    {
        return new DefaultAsyncMapInternal<TKey, TValue>(externalDataAllocator, this);
    }
}
