using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultOrderedKey : IOrderedKey<DefaultOrderedKey>
{
    private readonly IOrderedKeyBuilder<DefaultOrderedKey> keyBuilder;
    private readonly IKeyToStringHelper keyToStringHelper;

    public DefaultOrderedKey(IOrderedKeyBuilder<DefaultOrderedKey> keyBuilder,
                             IKeyToStringHelper keyToStringHelper,
                             IAggregationStructure aggregationStructure, 
                             IReadOnlyDictionary<AggregationLevel,string?> aggrigationLevelValues)
    {
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
        this.keyBuilder = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
        AggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof(aggregationStructure));
        argumentValidation(aggrigationLevelValues);
        StructureValues = aggregationStructure.Select(x => aggrigationLevelValues[x]!).ToArray();
    }

    public IAggregationStructure AggregationStructure { get; }
    public string[] StructureValues { get; }

    public bool Equals(IKey? other)
    {
        var otherKey = other as DefaultOrderedKey;
        if (otherKey == null) return false;
        if(otherKey.AggregationStructure != AggregationStructure) return false;
        if(StructureValues.SequenceEqual(otherKey.StructureValues)) return true;
        return false;
    }

    public DefaultOrderedKey GetSubKey(IAggregationStructure subAggregationStructure)
    {
        if(!subAggregationStructure.IsPrefixOf(AggregationStructure)) return keyBuilder.BuildEmptyKey();
        return keyBuilder.BuildSubKey(this, subAggregationStructure);
    }

    public bool IsEmpty()
    {
        return AggregationStructure.IsEmpty();  
    }

    public bool IsPrefixOf(DefaultOrderedKey otherKey)
    {
        return AggregationStructure.IsPrefixOf(otherKey.AggregationStructure);
    }

    public string ToUniqueString()
    {
        return keyToStringHelper.OrderKeyToStringKey(AggregationStructure.Select(x => x.ToString()).ToArray(), StructureValues);
    }

    private void argumentValidation(IReadOnlyDictionary<AggregationLevel, string?> aggrigationLevelValues)
    {
        var nullValues = AggregationStructure.Where(x => aggrigationLevelValues[x] == null);
        if (nullValues.Count() != 0)
        {
            throw new ArgumentException($"No value for aggregation levels {string.Join(",", nullValues)} are found.");
        }
    }
}
