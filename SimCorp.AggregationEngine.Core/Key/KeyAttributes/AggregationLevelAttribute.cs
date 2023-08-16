using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.KeyAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AggregationLevelAttribute : Attribute
{
    public AggregationLevel AggregationLevel { get; }
    public AggregationLevelAttribute(AggregationLevel aggregationLevel)
    {
        AggregationLevel = aggregationLevel;
    }
}
