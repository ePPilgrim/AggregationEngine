using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IDataLayerFactory<TValue>
{
    IAsyncMapInternal<TKey, TValue> Create<TKey>() where TKey : IKey;
}
