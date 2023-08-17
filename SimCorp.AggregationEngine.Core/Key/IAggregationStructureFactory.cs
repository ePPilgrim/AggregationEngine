using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IAggregationStructureFactory
{
    IAggregationStructure CreateAggregationStructure(IEnumerable<AggregationLevel> uniqueOrderedSequenceOfAggregationLevels);
    IAggregationStructure CreateEmptyAggregationStructure();
}
