using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IOrderedKeyBuilder<TKey> where TKey : class, IOrderedKey<TKey>
{
    TKey Build(IMetaData metaData);
    TKey BuildEmptyKey();
    TKey BuildSubKey(TKey Key, IAggregationStructure subAggregationStructure);
}
