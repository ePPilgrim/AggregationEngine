using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer;

internal interface IDataLayerFactory<TValue>
{
    IAsyncMapInternal<TKey, TValue> Create<TKey>() where TKey : IKey;
    IAsyncMapInternal<TKey, TValue> Create<TKey>(IAsyncExternalDataAllocator<TValue> externalDataAllocator) where TKey : IKey;
}
