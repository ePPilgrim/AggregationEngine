namespace AggregationEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false)]
public class AggregationLevelAttribute : Attribute
{
    public AggregationLevel AggregationLevel { get; }
    public AggregationLevelAttribute(AggregationLevel aggregationLevel)
    {
        AggregationLevel = aggregationLevel;
    }
}
