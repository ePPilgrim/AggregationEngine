using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultOrderedKeyBuilder : IOrderedKeyBuilder<DefaultOrderedKey> 
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    private readonly IOrderedKeyFactory<DefaultOrderedKey> keyFactory;
    private readonly IAggregationStructure aggregationStructure;
    public DefaultOrderedKeyBuilder(IKeyPropertySelector keyPropertySelector,
                                    IKeyToStringHelper keyToStringHelper,
                                    IOrderedKeyFactory<DefaultOrderedKey> keyFactory,
                                    IAggregationStructure aggregationStructure)
    {
        this.keyPropertySelector = keyPropertySelector ?? throw new ArgumentNullException(nameof(keyPropertySelector));
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException( nameof(keyToStringHelper));
        this.aggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof (aggregationStructure));
        this.keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
    }

    public DefaultOrderedKey Build<T>(T metaData)
    {
        var dict = new Dictionary<AggregationLevel, string?>();
        foreach (var aggregationLevel in aggregationStructure)
        {
            dict.Add(aggregationLevel, keyPropertySelector.GetPropertyWithAggregationLevel(metaData, aggregationLevel).Value);
        }
        return new DefaultOrderedKey(this, keyToStringHelper, aggregationStructure, dict);
    }

    public DefaultOrderedKey BuildEmptyKey()
    {
        return new DefaultOrderedKey(this, keyToStringHelper, keyFactory.CreateEmptyAggregationStructure(), new Dictionary<AggregationLevel, string?>());
    }

    public DefaultOrderedKey BuildSubKey(DefaultOrderedKey key, IAggregationStructure subAggregationStructure)
    {
        if (subAggregationStructure.IsPrefixOf(aggregationStructure))
        {
            return BuildEmptyKey();
        }
        var dict = new Dictionary<AggregationLevel, string?>();
        int index = 0;
        foreach(var aggregationLevel in subAggregationStructure)
        {
            dict.Add(aggregationLevel, key.StructureValues[index++]);
        }
        return new DefaultOrderedKey(this, keyToStringHelper, subAggregationStructure, dict);
    }
}
