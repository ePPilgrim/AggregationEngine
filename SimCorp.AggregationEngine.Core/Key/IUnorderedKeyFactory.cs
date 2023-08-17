namespace SimCorp.AggregationEngine.Core.Key;

public interface IUnorderedKeyFactory<TUnorderedKey> where TUnorderedKey : class, IKey
{
    IUnorderedKeyBuilder<TUnorderedKey> CreateUnorderedKeyBuilder();
}