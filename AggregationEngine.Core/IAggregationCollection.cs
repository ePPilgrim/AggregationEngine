using AggregationEngine.Core.Domain;

namespace AggregationEngine.Core;

public interface IAggregationCollection<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : IKey 
{
    IEnumerable<IAggregationCollection<TKey, TValue>> Decompose(AggregationLevel aggregationLevel);
    IEnumerable<IAggregationCollection<TKey, TValue>> DecomposeAndPopulate(AggregationLevel aggregationLevel);
    void AddLeaf(IAggregationPosition position, Action<TValue,IAggregationPosition> updateValue);
    void AddLeaves(IEnumerable<IAggregationPosition> positions, Action<TValue, IAggregationPosition> updateValue);
    void RemoveLeaf(TKey key);
    void RemoveLeaves(IEnumerable<TKey> keys);
    void ReconstructStructure(IAggregationStructure newAggregationStructure);
    void ApplyOperation(Action<TValue, IParameter, IAggregationPosition> updateValue);
    IEnumerable<IAggregationPosition> GetAggregationPositions(AggregationLevel aggregationLevel);
}
