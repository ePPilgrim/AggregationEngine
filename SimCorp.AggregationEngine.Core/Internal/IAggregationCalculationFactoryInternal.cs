using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                        where TUnorderedKey : IKey
                                                                                                        where TVector : IAggregationPosition
{
    IDelegateBuilderInternal<TOrderedKey, TVector, TResult> CreateDelegateBuilder();

}
