using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public interface IUnorderedKeyBuilder<TKey> where TKey : IKey
{
    TKey BuildForPositions<T>(T vector) where T : IMetaData;
    TKey BuildForParameters<T>(T parameters) where T : IParameters;
}
