namespace SimCorp.AggregationEngine.Core.Domain;

public interface IAggregationPosition 
{
    IMetaData MetaData { get; }
    IVector Values { get; }
}
