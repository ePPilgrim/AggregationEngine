using SimCorp.AggregationEngine.Core.Domain;
using System.Collections;
using System.Text;

namespace SimCorp.AggregationEngine.Core.Key.OrderedKey.AggregationStructure;

public class AggregationStructure : IAggregationStructure
{
    private readonly IAggregationStructureBuilder builder;
    private readonly List<AggregationLevel> structure;

    public AggregationStructure(IEnumerable<AggregationLevel> structuredUniqueAggregationLevels, IAggregationStructureBuilder builder)
    {
        this.builder = builder;
        structure = structuredUniqueAggregationLevels.ToList();
    }

    public bool Equals(IAggregationStructure? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return this.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IAggregationStructure);
    }

    public IEnumerator<AggregationLevel> GetEnumerator()
    {
        foreach (var item in structure)
        {
            yield return item;
        }
    }

    public IAggregationStructure GetSubStructureAt(AggregationLevel aggregationLevel)
    {
        if (structure.All(x => x != aggregationLevel))
        {
            return builder.BuildEmptyAggregationStructure();
        }
        int idex = structure.FindIndex(x => x == aggregationLevel);
        return builder.BuildFromSequence(Enumerable.Range(0, idex + 1).Select(pos => structure[pos]));
    }

    public bool IsEmpty()
    {
        return structure.Count == 0;
    }

    public override string ToString()
    {
        var strBuilding = new StringBuilder();
        strBuilding.Append("( ");
        foreach (var item in structure)
        {
            strBuilding.Append($" {item},");
        }
        strBuilding.Append(" )");
        return strBuilding.ToString();
    }

    public bool IsPrefixOf(IAggregationStructure aggregateStructure)
    {
        var castedAggregationStructure = aggregateStructure as AggregationStructure;
        if (ReferenceEquals(null, castedAggregationStructure)) return false;
        if (structure.Count > aggregateStructure.Count()) return false;
        if (IsEmpty()) return true;
        for (int i = structure.Count - 1; i >= 0; --i)
        {
            if (aggregateStructure[i] != structure[i]) return false;
        }

        return true;
    }
    public AggregationLevel this[int index] => structure[index];

    IEnumerator IEnumerable.GetEnumerator()
    {
        return structure.GetEnumerator();
    }
}
