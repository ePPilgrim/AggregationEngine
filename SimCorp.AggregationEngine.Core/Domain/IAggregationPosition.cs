namespace SimCorp.AggregationEngine.Core.Domain;

public interface IAggregationPosition : IMetaData, IScalingData, IVector
{
    IMetaData MetaData { get; }
    IScalingData ScalingData { get; }
    IVector Values { get; }
}
