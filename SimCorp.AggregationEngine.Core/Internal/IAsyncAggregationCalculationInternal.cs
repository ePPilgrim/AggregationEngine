using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult>   where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                        where TUnorderedKey : IKey
                                                                                                        where TVector : IMetaData
{
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
