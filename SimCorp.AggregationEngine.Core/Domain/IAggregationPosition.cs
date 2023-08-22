namespace SimCorp.AggregationEngine.Core.Domain;

public interface IAggregationPosition : IMetaData
{
    void DoAdditionOperation(IAggregationPosition aggregationPosition);
}
