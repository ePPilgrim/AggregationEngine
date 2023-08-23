using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core;

public interface IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                            where TUnorderedKey : IKey
                                                                                            where TVector : IAggregationPosition
{
    void SetAggregationStructure(IEnumerable<AggregationLevel> aggregationStructure);
    Task UpdateOrAddAsync(IEnumerable<TVector> positions, CancellationToken token);
    Task RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token);
    IDictionary<TUnorderedKey, IMetaData> GetAllLeaves();
    Task<IDictionary<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token);
    Task<IDictionary<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token);
    Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, CancellationToken token);
}
