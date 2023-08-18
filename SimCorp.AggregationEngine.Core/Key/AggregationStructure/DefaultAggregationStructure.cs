using SimCorp.AggregationEngine.Core.Domain;
using System.Collections;

namespace SimCorp.AggregationEngine.Core.Key.AggregationStructure;

public class DefaultAggregationStructure : IAggregationStructure
{
    private readonly IAggregationStructureBuilder builder;
    private readonly Stack<AggregationLevel> stack;

    public DefaultAggregationStructure(Stack<AggregationLevel> structuredUniqueAggregationLevels, IAggregationStructureBuilder builder) 
    {
        this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
        stack = new Stack<AggregationLevel>();
        if (structuredUniqueAggregationLevels.ToHashSet().Count() != structuredUniqueAggregationLevels.Count)
        {
            throw new ArgumentException($"Aggregation levels must be unique in the aggregation structure.");
        }
        if (structuredUniqueAggregationLevels.Any(x => x == AggregationLevel.None))
        {
            throw new ArgumentException($"Aggregation structure must not contain None aggregation level.");
        }
        stack = structuredUniqueAggregationLevels;
    }

    public bool Equals(IAggregationStructure? other)
    {
        if (!ReferenceEquals(this, other)) return false;
        return this.SequenceEqual(other);
    }

    public IEnumerator<AggregationLevel> GetEnumerator()
    {
        return stack.Reverse().GetEnumerator();
    }

    public IAggregationStructure GetSubStructureAt(AggregationLevel aggregationLevel)
    {
        var subStructure = builder.BuildFromSequence(this);
        foreach (var level in subStructure)
        {
            if (level == subStructure.Peek()) break;
            subStructure.Pop();
        }
        return subStructure;
    }

    public bool IsEmpty()
    {
        return stack.Count == 0;
    }

    public bool IsPrefixOf(IAggregationStructure aggregateStructure)
    {
        if (!ReferenceEquals(this, aggregateStructure)) return false;
        if (stack.Count > aggregateStructure.Count()) return false;
        if (IsEmpty()) return true;
        int index = stack.Count - 1;
        var stackArray = stack.ToArray();
        foreach (var item in aggregateStructure)
        {
            if (index < 0) break;
            if (stackArray[index--] != item) return false;
        }
        return true;
    }

    public AggregationLevel Peek()
    {
        return stack.Peek();
    }

    public AggregationLevel Pop()
    {
        return stack.Pop();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return stack.GetEnumerator();
    }
}
