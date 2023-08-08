namespace AggregationEngine.Core.Domain;

public interface IKey : IEquatable<IKey> //Multylevel key
{
    bool IsPrefixOf(IKey otherKey);
    IKey GetSubKey(int levelIndex);
    bool IsEmpty { get; }
}
