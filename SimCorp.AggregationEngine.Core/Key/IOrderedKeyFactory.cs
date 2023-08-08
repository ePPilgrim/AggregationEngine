using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IOrderedKeyFactory<TOrderedKey, TVector> where TOrderedKey : IKey
                                                          where TVector : struct, IAggregationPosition
{
    IKeyBuilder<TOrderedKey> CreateKeyBuilder(TVector vector);
    IKeyOperations<TOrderedKey> CreateKeyOperations();
}
