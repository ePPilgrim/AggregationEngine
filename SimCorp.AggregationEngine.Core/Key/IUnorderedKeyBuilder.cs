using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IUnorderedKeyBuilder<TKey> where TKey : IKey
{
    TKey Build(IMetaData metaData);
    TKey Build(IParameters parameters);
}
