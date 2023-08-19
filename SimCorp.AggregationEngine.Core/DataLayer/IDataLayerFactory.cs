using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IDataLayerFactory<TValue>
{
    IAsyncMapInternal<TKey, TValue> Create<TKey>() where TKey : IKey;
    IAsyncMapInternal<TKey, TValue> Create<TKey>(IAsyncExternalDataAllocator<TValue> externalDataAllocator) where TKey : IKey;
    //IAsyncMapVectorWrapperInternal<TKey, TValue> CreateVectorWrapper<TKey>(IAsyncExternalDataAllocator<TValue> externalDataAllocator) where TKey : IKey where TValue : IAggregationPosition;
}
