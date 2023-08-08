namespace AggregationEngine.Core;

public interface IAggregationPosition
{
    IAggregationPosition DoAdditionWith(IAggregationPosition otherAggregationPosition);
}
