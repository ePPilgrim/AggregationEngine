using SimCorp.AggregationEngine.Core.Key.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core.Key;

public class DefaultKeyFactory : IKeyFactory<DefaultOrderedKey, DefaultUnorderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    public DefaultKeyFactory(IKeyPropertySelector keyPropertySelector, IKeyToStringHelper keyToStringHelper)
    {
        this.keyPropertySelector = keyPropertySelector ?? throw new ArgumentNullException(nameof(keyPropertySelector));
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
    }
    public IAggregationStructureBuilder CreateAggregationStructureBuilder()
    {
        return new DefaultAggregatonStructureBuilder();
    }

    public IOrderedKeyBuilder<DefaultOrderedKey> CreateOrderedKeyBuilder(IAggregationStructure aggregationStructure)
    {
        return new DefaultOrderedKeyBuilder(keyPropertySelector, keyToStringHelper, CreateAggregationStructureBuilder(), aggregationStructure);
    }

    public IUnorderedKeyBuilder<DefaultUnorderedKey> CreateUnorderedKeyBuilder()
    {
        return new DefaultUnorderedKeyBuilder(keyPropertySelector, keyToStringHelper);
    }
}
