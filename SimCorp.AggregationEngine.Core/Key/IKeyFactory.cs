using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.OrderedKey.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IKeyFactory<TOrderedKey, TUnorderedKey> : IOrderedKeyFactory<TOrderedKey>, IUnorderedKeyFactory<TUnorderedKey>, IAggregationStructureFactory 
    where TOrderedKey : IOrderedKey<TOrderedKey> 
    where TUnorderedKey : IKey
{}
