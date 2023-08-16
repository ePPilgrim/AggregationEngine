using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core;

public interface IAggregationStructure : IEnumerable<AggregationLevel>, IEquatable<IAggregationStructure> 
{
    bool IsPrefixOf(IAggregationStructure aggregateStructure);
    bool IsEmpty();
}
