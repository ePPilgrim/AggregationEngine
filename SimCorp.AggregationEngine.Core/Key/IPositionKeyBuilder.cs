using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IPositionKeyBuilder<TKey> where TKey : IKey
{
    TKey Build(IMetaData metaData);
}
