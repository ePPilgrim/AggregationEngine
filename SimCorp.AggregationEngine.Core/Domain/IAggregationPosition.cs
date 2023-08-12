namespace SimCorp.AggregationEngine.Core.Domain;

public interface IAggregationPosition : IEquatable<IAggregationPosition>
{
    void CombineWith(IEnumerable<IAggregationPosition> aggregationPositions);
}
