using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAsyncMapVectorDecoratorInternal<TKey, TVector> : IAsyncMapInternal<TKey, AllocatorWrapperInternal<TVector>> where TKey : IKey
                                                                                                                                where TVector : IAggregationPosition                                                                                    
{
    Task<AllocatorWrapperInternal<TVector>> AddAsync(TKey key, TVector wrappedPosition, CancellationToken token);
}
