using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey.AggregationStructure;

public interface IAggregationStructure : IEnumerable<AggregationLevel>, IEquatable<IAggregationStructure>
{
    bool IsPrefixOf(IAggregationStructure aggregateStructure);
    bool IsEmpty();
    IAggregationStructure GetSubStructureAt(AggregationLevel aggregationLevel);
    AggregationLevel this[int index] { get; }
}
