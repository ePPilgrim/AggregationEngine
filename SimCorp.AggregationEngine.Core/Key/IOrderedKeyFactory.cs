namespace SimCorp.AggregationEngine.Core.Key;

public interface IOrderedKeyFactory<TOrderedKey> : IAggregationStructureFactory where TOrderedKey : class, IOrderedKey<TOrderedKey>
{
    IOrderedKeyBuilder<TOrderedKey> CreateOrderedKeyBuilder(IAggregationStructure aggregationStructure);
}
