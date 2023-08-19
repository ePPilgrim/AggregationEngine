using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public interface IUnorderedKeyBuilder<TKey> where TKey : IKey
{
    TKey BuildForPositions<T>(T vector) where T : IMetaData;
    TKey BuildForParameters<T>(T parameters) where T : IParameters;
}
