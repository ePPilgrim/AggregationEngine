using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer;

internal interface IPositionDataLayerFactory<TVector> : IDataLayerFactory<TVector> where TVector : IAggregationPosition
{
    IAsyncMapVectorWrapperInternal<TKey, TVector> Create<TKey>(IAsyncMapInternal<TKey, TVector> dataAllocator) where TKey : IKey;
}
