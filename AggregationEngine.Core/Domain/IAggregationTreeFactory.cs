namespace AggregationEngine.Core.Domain;

public interface IAggregationTreeFactory<TKey, TValue> where TKey : IKey
{
    INode<TKey, TValue> CreateNode();
    TKey CreateEmptyKey();
    IKeyBuilder<TKey> CreateKeyBuilder(IEnumerable<IAggregationPosition> position);
    IKeyBuilder<TKey> CreateKeyBuilder(IAggregationPosition position);
}