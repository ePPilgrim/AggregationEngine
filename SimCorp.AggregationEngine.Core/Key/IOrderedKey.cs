namespace SimCorp.AggregationEngine.Core.Key;

public interface IOrderedKey<TKey> : IKey where TKey : class
{
    bool IsPrefixOf(TKey otherKey);
    bool IsEmpty();
    TKey GetSubKey(IAggregationStructure subAggregationStructure);
}
