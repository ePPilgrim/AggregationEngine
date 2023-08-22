using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public class DefaultUnorderedKeyBuilder : IUnorderedKeyBuilder<DefaultUnorderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    public DefaultUnorderedKeyBuilder(IKeyPropertySelector keyPropertySelector, IKeyToStringHelper keyToStringHelper)
    {
        this.keyPropertySelector = keyPropertySelector;
        this.keyToStringHelper = keyToStringHelper;
    }

    public DefaultUnorderedKey BuildForParameters<T>(T parameters) where T : IParameters
    {
        return new DefaultUnorderedKey(keyToStringHelper, keyPropertySelector.GetPropertiesWithKeyAttribute(parameters));
    }

    public DefaultUnorderedKey BuildForPositions<T>(T vector) where T : IMetaData
    {
        var dict = new Dictionary<string, string?>();
        foreach (var aggregationLevel in (AggregationLevel[])Enum.GetValues(typeof(AggregationLevel)))
        {
            if (aggregationLevel != AggregationLevel.None && aggregationLevel != AggregationLevel.Top)
            {
                var item = keyPropertySelector.GetPropertyWithAggregationLevel<IMetaData>(vector, aggregationLevel);
                dict.Add(item.Key, item.Value);
            }
        }
        return new DefaultUnorderedKey(keyToStringHelper, dict);
    }
}
