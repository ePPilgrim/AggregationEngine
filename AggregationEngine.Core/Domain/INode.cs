namespace AggregationEngine.Core.Domain;

public interface INode<TKey, TValue> where TKey : IKey
{
    public TValue Value { get; }
    public TKey ParentKey { get; set; }
    public HashSet<TKey> ChildrenKeys { get; }
}
