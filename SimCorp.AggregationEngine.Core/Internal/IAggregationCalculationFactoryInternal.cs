using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>
                                                                                                        where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                        where TUnorderedKey : IKey
                                                                                                        where TVector : IAggregationPosition
{
    IAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, IVectorAllocatorWrapperInternal<TVector>, TResult> CreateAggregationCalculationBuilder();
    IDelegateBuilderInternal<TOrderedKey, TVector, TResult> CreateDelegateBuilder(Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                                  Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator);
    IAsyncMapInternal<TKey, TVector> CreatePositionAllocator<TKey>() where TKey : IKey;
    IAsyncMapVectorWrapperInternal<TKey, TVector> CreateWrapperPositionAllocator<TKey>() where TKey : IKey;
    IAsyncMapInternal<TUnorderedKey, TResult> CreateResultAllocator();
}
