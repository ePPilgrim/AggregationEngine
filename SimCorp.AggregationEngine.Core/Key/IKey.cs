namespace SimCorp.AggregationEngine.Core.Key;

public interface IKey : IEquatable<IKey> //Multylevel ordered key
{
    bool IsPrefixOf(IKey otherKey);
    bool IsEmpty { get; }
    int NumberOfSubKeys { get; }
    IAggregationStructure AggregationStructure { get; }
}
