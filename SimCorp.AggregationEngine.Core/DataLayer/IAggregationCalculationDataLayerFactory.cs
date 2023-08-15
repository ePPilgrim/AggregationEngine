using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAggregationCalculationDataLayerFactory<TVector, TResult> where TVector : struct, IAggregationPosition
{
    IAsyncMap<TKey, TVector> CreateEmptyVectorMap<TKey>() where TKey : IEquatable<TKey>;
    IAsyncMap<TKey, TResult> CreateEmptyResultMap<TKey>() where TKey : IEquatable<TKey>;
    VectorWrapper<TVector> CreateVectorWithAllocatorLayer(TVector vector);
    ICacheInternal<TResult> CreateResultCache();
}
