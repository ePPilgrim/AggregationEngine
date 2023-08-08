namespace AggregationEngine.Core.Domain;

public interface IKeyBuilder<TKey> where TKey : IKey
{
    IKeyBuilder<TKey> Add(AggregationLevel aggregationLevel);
    IKeyBuilder<TKey> Add(IAggregationStructure aggregationStructure);
    IEnumerable<TKey> Build();
}
