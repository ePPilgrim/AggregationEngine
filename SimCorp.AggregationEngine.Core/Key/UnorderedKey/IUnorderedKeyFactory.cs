namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public interface IUnorderedKeyFactory<TUnorderedKey> where TUnorderedKey : IKey
{
    IUnorderedKeyBuilder<TUnorderedKey> CreateUnorderedKeyBuilder();
}