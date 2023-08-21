using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer;

internal interface IAsyncMapVectorWrapperInternal<TKey, TVector> : IAsyncMapInternal<TKey, IVectorAllocatorWrapperInternal<TVector>> where TKey : IKey
                                                                                                                                        where TVector : IAggregationPosition
{
    Task<IVectorAllocatorWrapperInternal<TVector>> AddAsync(TKey key, TVector wrappedPosition, CancellationToken token);
}
