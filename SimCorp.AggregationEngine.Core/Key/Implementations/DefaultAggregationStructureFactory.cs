using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultAggregationStructureFactory : IAggregationStructureFactory
{
    public IAggregationStructure CreateAggregationStructure(IEnumerable<AggregationLevel> uniqueOrderedSequenceOfAggregationLevels)
    {
        return new DefaultAggregationStructure(new Stack<AggregationLevel>(uniqueOrderedSequenceOfAggregationLevels), this);
    }

    public IAggregationStructure CreateEmptyAggregationStructure()
    {
        return new DefaultAggregationStructure(this);
    }
}
