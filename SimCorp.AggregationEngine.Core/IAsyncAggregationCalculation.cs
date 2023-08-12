using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core;

public interface IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IKey
                                                                                                    where TUnorderedKey : IEqualityComparer<TUnorderedKey>
                                                                                                    where TVector : struct, IAggregationPosition
{
    Task UpdateOrAddAsync(IEnumerable<TVector> positions, CancellationToken token);
    Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token);
    Task<IDictionary<TUnorderedKey, TVector>> GetAllLeaves();
    Task<IDictionary<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token);
    Task<IDictionary<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token);
    Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, CancellationToken token);
}
