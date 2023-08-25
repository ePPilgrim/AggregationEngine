using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public class UnorderedKeyBuilder : IUnorderedKeyBuilder<UnorderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    public UnorderedKeyBuilder(IKeyPropertySelector keyPropertySelector, IKeyToStringHelper keyToStringHelper)
    {
        this.keyPropertySelector = keyPropertySelector;
        this.keyToStringHelper = keyToStringHelper;
    }

    public UnorderedKey BuildForParameters<T>(T parameters) where T : IParameters
    {
        return new UnorderedKey(keyToStringHelper, keyPropertySelector.GetPropertiesWithKeyAttribute(parameters));
    }

    public UnorderedKey BuildForPositions<T>(T vector) where T : IMetaData
    {
        var dict = new Dictionary<string, string?>();
        foreach (var aggregationLevel in (AggregationLevel[])Enum.GetValues(typeof(AggregationLevel)))
        {
            if (aggregationLevel != AggregationLevel.None && aggregationLevel != AggregationLevel.Top)
            {
                var item = keyPropertySelector.GetPropertyWithAggregationLevel<IMetaData>(vector, aggregationLevel);
               if(item != null)
                {
                    dict.Add(item.Value.Key, item.Value.Value);
                }
                
            }
        }
        return new UnorderedKey(keyToStringHelper, dict);
    }
}
