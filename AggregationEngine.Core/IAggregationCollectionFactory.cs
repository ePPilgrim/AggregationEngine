using AggregationEngine.Core.Domain;

namespace AggregationEngine.Core
{
    public interface IAggregationCollectionFactory<TKey, TValue> where TKey : IKey
    {
        INode<TKey, TValue> CreateNode(AggregationLevel aggregationLevel);
        TKey CreateEmptyKey();
        IKeyBuilder<TKey> CreateKeyBuilder(IAggregationPosition position);
    }
}
