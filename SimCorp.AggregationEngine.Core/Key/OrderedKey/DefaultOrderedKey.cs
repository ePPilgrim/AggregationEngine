using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey;

public class DefaultOrderedKey : IOrderedKey<DefaultOrderedKey>
{
    private readonly IOrderedKeyBuilder<DefaultOrderedKey> keyBuilder;
    private readonly IKeyToStringHelper keyToStringHelper;

    public DefaultOrderedKey(IOrderedKeyBuilder<DefaultOrderedKey> keyBuilder,
                             IKeyToStringHelper keyToStringHelper,
                             IAggregationStructure aggregationStructure,
                             IReadOnlyDictionary<AggregationLevel, string?> aggrigationLevelValues)
    {
        this.keyToStringHelper = keyToStringHelper;
        this.keyBuilder = keyBuilder;
        AggregationStructure = aggregationStructure;
        argumentValidation(aggrigationLevelValues);
        StructureValues = aggregationStructure.Select(x => aggrigationLevelValues[x]!).ToArray();
    }

    public IAggregationStructure AggregationStructure { get; }
    public string[] StructureValues { get; }

    public bool Equals(IKey? other)
    {
        var otherKey = other as DefaultOrderedKey;
        if (otherKey == null) return false;
        if (otherKey.AggregationStructure != AggregationStructure) return false;
        if (StructureValues.SequenceEqual(otherKey.StructureValues)) return true;
        return false;
    }

    public override int GetHashCode()
    {
        return ToUniqueString().GetHashCode();
    }

    public DefaultOrderedKey GetSubKey(IAggregationStructure subAggregationStructure)
    {
        return keyBuilder.BuildSubKey(this, subAggregationStructure);
    }

    public bool IsEmpty()
    {
        return AggregationStructure.IsEmpty();
    }

    public bool IsPrefixOf(DefaultOrderedKey otherKey)
    {
        if(StructureValues.Length > otherKey.StructureValues.Length) return false;  
        for(int i = StructureValues.Length - 1; i >= 0; i--)
        {
            if (AggregationStructure[i] != otherKey.AggregationStructure[i]) return false;
            if (StructureValues[i] != otherKey.StructureValues[i]) return false;
        }
        return true;
    }

    public string ToUniqueString()
    {
        return keyToStringHelper.OrderKeyToStringKey(AggregationStructure.Select(x => x.ToString()).ToArray(), StructureValues);
    }

    public override string ToString()
    {
        return ToUniqueString();
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
