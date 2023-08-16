namespace SimCorp.AggregationEngine.Core.Key;

public interface IKeyFactory<TOrderedKey, TUnorderedKey> where TOrderedKey : class, IOrderedKey<TOrderedKey>
                                                         where TUnorderedKey : class, IKey
{
    IOrderedKeyBuilder<TOrderedKey> CreateOrderedKeyBuilder();
    IUnorderedKeyBuilder<TUnorderedKey> CreateUnorderedKeyBuilder();
}
