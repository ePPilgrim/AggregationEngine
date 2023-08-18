using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.AggregationStructure;

public interface IAggregationStructureBuilder
{
    IAggregationStructure BuildFromSequence(IEnumerable<AggregationLevel> uniqueOrderedSequenceOfAggregationLevels);
    IAggregationStructure BuildEmptyAggregationStructure();
}
