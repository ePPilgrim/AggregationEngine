using SimCorp.AggregationEngine.Core.DataCollections;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IEmptyMapFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IKey
                                                                                                        where TUnorderedKey : IEqualityComparer<TUnorderedKey>
                                                                                                        where TVector : struct, IAggregationPosition
{
    IMap<TUnorderedKey, TVector> CreateEmptyUnorderedVectorMap();
    IMap<TUnorderedKey, TResult> CreateEmptyUnorderedResultMap();
    IMap<TOrderedKey, IMap<TUnorderedKey, TResult>> CreateEmptyOrderedUnorderedResultMap();
}
