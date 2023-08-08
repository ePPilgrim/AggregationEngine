namespace SimCorp.AggregationEngine.Core.Key;

public interface IKeyOperations<TKey> where TKey : IKey
{
    TKey SubKey(TKey key, IAggregationStructure subAggregationStructure);
}
