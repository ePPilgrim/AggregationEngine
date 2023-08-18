using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult>   where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                        where TUnorderedKey : IKey
                                                                                                        where TVector : IAggregationPosition
{
    IAsyncMapInternal<TUnorderedKey, TVector> GetAllLeaves();
    IAggregationStructure AggregartionStructure { get; set; }
    Task<IAsyncMapInternal<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey,
                                                     IDictionary<TUnorderedKey, IParameters> parameters,
                                                     Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                     Func<IEnumerable<TVector>, TOrderedKey, CancellationToken, Task<TVector>> accumulator,
                                                     CancellationToken token);
    Task<IAsyncMapInternal<DualKey<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey,
                                                                        IDictionary<TUnorderedKey, IParameters> parameters,
                                                                        Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                        Func<IEnumerable<TVector>, TOrderedKey, CancellationToken, Task<TVector>> accumulator,
                                                                        CancellationToken token);
    Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, 
        Func<IEnumerable<TVector>, TOrderedKey, CancellationToken, Task<TVector>> accumulator,
        CancellationToken token);
}
