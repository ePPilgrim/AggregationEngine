using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IKey
                                                                                                    where TUnorderedKey : IEqualityComparer<TUnorderedKey>
                                                                                                    where TVector : struct, IAggregationPosition
{
    Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TUnorderedKey, TVector>> positions, CancellationToken token);
    Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token);
    IAsyncMap<TUnorderedKey, TVector> GetAllLeaves();
    Task<IAsyncMap<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey,
                                                     IDictionary<TUnorderedKey, IParameters> parameters,
                                                     Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                     Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator,
                                                     CancellationToken token);
    Task<IAsyncMap<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey,
                                                                        IDictionary<TUnorderedKey, IParameters> parameters,
                                                                        Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                        Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator,
                                                                        CancellationToken token);
    Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, 
        Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator,
        CancellationToken token);
}
