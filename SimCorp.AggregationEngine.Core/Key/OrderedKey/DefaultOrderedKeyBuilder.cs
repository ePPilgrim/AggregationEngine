using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey;

public class DefaultOrderedKeyBuilder : IOrderedKeyBuilder<DefaultOrderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    private readonly IAggregationStructureBuilder aggregationStructureBuilder;
    private readonly IAggregationStructure aggregationStructure;
    public DefaultOrderedKeyBuilder(IKeyPropertySelector keyPropertySelector,
                                    IKeyToStringHelper keyToStringHelper,
                                    IAggregationStructureBuilder aggregationStructureBuilder,
                                    IAggregationStructure aggregationStructure)
    {
        this.keyPropertySelector = keyPropertySelector;
        this.keyToStringHelper = keyToStringHelper;
        this.aggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof(aggregationStructure));
        this.aggregationStructureBuilder = aggregationStructureBuilder ?? throw new ArgumentNullException(nameof(aggregationStructureBuilder));
    }

    public DefaultOrderedKey Build<T>(T metaData) where T : IMetaData
    {
        var dict = new Dictionary<AggregationLevel, string?>();
        foreach (var aggregationLevel in aggregationStructure)
        {
            if(aggregationLevel == AggregationLevel.Top)
            {
                dict.Add(AggregationLevel.Top, "All");
                continue;
            }
            dict.Add(aggregationLevel, keyPropertySelector.GetPropertyWithAggregationLevel<IMetaData>(metaData, aggregationLevel).Value);
        }
        return new DefaultOrderedKey(this, keyToStringHelper, aggregationStructure, dict);
    }

    public DefaultOrderedKey BuildEmptyKey()
    {
        return new DefaultOrderedKey(this, keyToStringHelper, aggregationStructureBuilder.BuildEmptyAggregationStructure(), new Dictionary<AggregationLevel, string?>());
    }

    public DefaultOrderedKey BuildSubKey(DefaultOrderedKey key, IAggregationStructure subAggregationStructure)
    {
        if (!subAggregationStructure.IsPrefixOf(aggregationStructure))
        {
            return BuildEmptyKey();
        }
        var dict = new Dictionary<AggregationLevel, string?>();
        int index = 0;
        foreach (var aggregationLevel in subAggregationStructure)
        {
            dict.Add(aggregationLevel, key.StructureValues[index++]);
        }
        return new DefaultOrderedKey(this, keyToStringHelper, subAggregationStructure, dict);
    }
}
