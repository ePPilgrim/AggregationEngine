using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey;

public interface IOrderedKeyBuilder<TKey> where TKey : IOrderedKey<TKey>
{
    TKey Build<T>(T metaData) where T : IMetaData;
    TKey BuildEmptyKey();
    TKey BuildSubKey(TKey key, IAggregationStructure subAggregationStructure);
}
