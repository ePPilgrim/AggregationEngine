using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IAggregationStructure : IEnumerable<AggregationLevel>, IEquatable<IAggregationStructure>
{
    bool IsPrefixOf(IAggregationStructure aggregateStructure);
    bool IsEmpty();
    AggregationLevel Pop();
    AggregationLevel Peek();
    IAggregationStructure GetSubStructureAt(AggregationLevel aggregationLevel);
}
