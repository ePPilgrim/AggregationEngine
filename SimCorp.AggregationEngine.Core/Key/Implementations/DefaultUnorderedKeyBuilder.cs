using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultUnorderedKeyBuilder : IUnorderedKeyBuilder<DefaultUnorderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    public DefaultUnorderedKeyBuilder(  IKeyPropertySelector keyPropertySelector,
                                        IKeyToStringHelper keyToStringHelper)
    {
        this.keyPropertySelector = keyPropertySelector ?? throw new ArgumentNullException(nameof(keyPropertySelector));
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
    }

    public DefaultUnorderedKey BuildForParameters<TParam>(TParam parameters)
    {
        return new DefaultUnorderedKey(keyToStringHelper, keyPropertySelector.GetPropertiesWithKeyAttribute(parameters));
    }

    public DefaultUnorderedKey BuildForVectors<TVector>(TVector vector)
    {
        var dict = new Dictionary<string, string?>();
        foreach (var aggregationLevel in (AggregationLevel[])Enum.GetValues(typeof(AggregationLevel)))
        {
            var item = keyPropertySelector.GetPropertyWithAggregationLevel(vector, aggregationLevel);
            dict.Add(item.Key, item.Value);
        }
        return new DefaultUnorderedKey(keyToStringHelper, dict);
    }
}
