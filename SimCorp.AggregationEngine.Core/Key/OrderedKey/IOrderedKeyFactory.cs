using SimCorp.AggregationEngine.Core.Key.OrderedKey.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey;

public interface IOrderedKeyFactory<TOrderedKey> : IAggregationStructureFactory where TOrderedKey : IOrderedKey<TOrderedKey>
{
    IOrderedKeyBuilder<TOrderedKey> CreateOrderedKeyBuilder(IAggregationStructure aggregationStructure);
}
