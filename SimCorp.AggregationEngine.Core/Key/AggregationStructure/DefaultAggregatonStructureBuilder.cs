using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.AggregationStructure;

public class DefaultAggregatonStructureBuilder : IAggregationStructureBuilder
{
    public IAggregationStructure BuildEmptyAggregationStructure()
    {
        return BuildFromSequence(new List<AggregationLevel>());
    }

    public IAggregationStructure BuildFromSequence(IEnumerable<AggregationLevel> uniqueOrderedSequenceOfAggregationLevels)
    {
        return new DefaultAggregationStructure(new Stack<AggregationLevel>(uniqueOrderedSequenceOfAggregationLevels), this);
    }
}
