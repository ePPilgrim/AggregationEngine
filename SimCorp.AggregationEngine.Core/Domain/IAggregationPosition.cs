namespace SimCorp.AggregationEngine.Core.Domain;

public interface IAggregationPosition : IEqualityComparer<IAggregationPosition>
{
    void CombineWith(IEnumerable<IAggregationPosition> aggregationPositions);
}
