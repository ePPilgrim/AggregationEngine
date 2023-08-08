using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core;

public interface IAggregationStructure : IEnumerable<IAggregationStructure>, IEquatable<IAggregationStructure>
{
    void Pop();
    void Push(AggregationLevel aggregationLevel);
    AggregationLevel Peek();
}
