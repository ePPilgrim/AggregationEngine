using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public interface IUnorderedKeyBuilder<TKey> where TKey : IKey
{
    TKey BuildForVectors<TVector>(TVector vector);
    TKey BuildForParameters<TParam>(TParam parameters);
}
