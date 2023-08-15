namespace SimCorp.AggregationEngine.Core.Key;

public interface IOrderedKey : IKey 
{
    bool IsPrefixOf(IKey otherKey);
    IAggregationStructure AggregationStructure { get; }
}
