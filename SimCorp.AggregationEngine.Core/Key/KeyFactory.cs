using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.OrderedKey.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core.Key;

public class KeyFactory : IKeyFactory<OrderedKey.OrderedKey, UnorderedKey.UnorderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    public KeyFactory(IKeyPropertySelector keyPropertySelector, IKeyToStringHelper keyToStringHelper)
    {
        this.keyPropertySelector = keyPropertySelector ?? throw new ArgumentNullException(nameof(keyPropertySelector));
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
    }
    public IAggregationStructureBuilder CreateAggregationStructureBuilder()
    {
        return new AggregatonStructureBuilder();
    }

    public IOrderedKeyBuilder<OrderedKey.OrderedKey> CreateOrderedKeyBuilder(IAggregationStructure aggregationStructure)
    {
        return new OrderedKeyBuilder(keyPropertySelector, keyToStringHelper, CreateAggregationStructureBuilder(), aggregationStructure);
    }

    public IUnorderedKeyBuilder<UnorderedKey.UnorderedKey> CreateUnorderedKeyBuilder()
    {
        return new UnorderedKeyBuilder(keyPropertySelector, keyToStringHelper);
    }
}
