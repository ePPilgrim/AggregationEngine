namespace SimCorp.AggregationEngine.Core.Key.Common;

public abstract class AbstractKey : IKey
{
    public abstract bool Equals(IKey? other);
    public abstract string ToUniqueString();
    public sealed override int GetHashCode() => ToUniqueString().GetHashCode();
    public sealed override bool Equals(object? obj) => Equals(obj as IKey);
    public override string ToString() => ToUniqueString();
}
