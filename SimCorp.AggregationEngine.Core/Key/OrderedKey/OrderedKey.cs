using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey;

public class OrderedKey : AbstractKey, IOrderedKey<OrderedKey>
{
    private readonly IOrderedKeyBuilder<OrderedKey> keyBuilder;
    private readonly IKeyToStringHelper keyToStringHelper;

    public OrderedKey(IOrderedKeyBuilder<OrderedKey> keyBuilder,
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

    public override bool Equals(IKey? other)
    {
        if (other == null) return false;
        return ToUniqueString() == other.ToUniqueString();
    }

    public OrderedKey GetSubKey(IAggregationStructure subAggregationStructure)
    {
        return keyBuilder.BuildSubKey(this, subAggregationStructure);
    }

    public bool IsEmpty()
    {
        return AggregationStructure.IsEmpty();
    }

    public bool IsPrefixOf(OrderedKey otherKey)
    {
        if(StructureValues.Length > otherKey.StructureValues.Length) return false;  
        for(int i = StructureValues.Length - 1; i >= 0; i--)
        {
            if (AggregationStructure[i] != otherKey.AggregationStructure[i]) return false;
            if (StructureValues[i] != otherKey.StructureValues[i]) return false;
        }
        return true;
    }

    public override string ToUniqueString()
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
