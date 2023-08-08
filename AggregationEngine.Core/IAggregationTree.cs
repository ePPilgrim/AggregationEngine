using AggregationEngine.Core.Domain;

namespace AggregationEngine.Core;

public interface IAggregationTree<TKey, TValue, TLevelKey> : IDictionary<TKey, TValue>
{
    HashSet<object> GetLevelKey(AggregationLevel aggregationLevel);
    void Evaluate(Action<TValue,IAggregationPosition> evaluator);
    IEnumerable<IAggregationTree<TKey, TValue, TLevelKey>> DecomposeBottom(AggregationLevel level, Action<TValue, IAggregationPosition> evaluator);
    void Reshuffle(IAggregationStructure reshuffledStructure, Action<TValue, IAggregationPosition> evaluator);

}
