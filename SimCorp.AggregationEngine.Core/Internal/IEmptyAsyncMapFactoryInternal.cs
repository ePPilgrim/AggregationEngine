using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IEmptyAsyncMapFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IKey
                                                                                               where TUnorderedKey : IEqualityComparer<TUnorderedKey>
                                                                                               where TVector : struct, IAggregationPosition
{
    IAsyncMap<TUnorderedKey, TVector> CreateEmptyUnorderedVectorAsyncMap();
    IAsyncMap<TUnorderedKey, TResult> CreateEmptyUnorderedResultAsyncMap();
    IAsyncMap<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult> CreateEmptyOrderedUnorderedResultAsyncMap();
    TVector CreateZeroVector();
}