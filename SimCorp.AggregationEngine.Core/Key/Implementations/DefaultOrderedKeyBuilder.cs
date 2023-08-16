using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultOrderedKeyBuilder<TKey> : IOrderedKeyBuilder<TKey> where TKey : class, IOrderedKey<TKey>
{
    private readonly IKeyInterfacePropertySelector keyPropertySelector;

    public DefaultOrderedKeyBuilder()
    {
        
    }
    public TKey Build(IMetaData metaData)
    {
        throw new NotImplementedException();
    }

    public TKey BuildEmptyKey()
    {
        throw new NotImplementedException();
    }

    public TKey BuildSubKey(TKey Key, IAggregationStructure subAggregationStructure)
    {
        throw new NotImplementedException();
    }
}
