using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IKeyBuilder<TKey> where TKey : IKey
{
    IKeyBuilder<TKey> Add(AggregationLevel aggregationLevel);
    IKeyBuilder<TKey> Add(IAggregationStructure aggregationStructure);
    TKey Build();
}
