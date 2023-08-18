using SimCorp.AggregationEngine.Core.Key.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey;

public interface IOrderedKey<TKey> : IKey where TKey : IKey
{
    bool IsPrefixOf(TKey otherKey);
    bool IsEmpty();
    TKey GetSubKey(IAggregationStructure subAggregationStructure);
}
