using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IDelegateBuilderInternal<TKey, TVector, TResult> : IDisposable   where TKey : IOrderedKey<TKey>
                                                                                    where TVector : IAggregationPosition
{
    Func<IVectorAllocatorWrapperInternal<TVector>, IParameters, CancellationToken, Task<TResult>> BuildCalculator();
    Func<IEnumerable<IVectorAllocatorWrapperInternal<TVector>>, TKey, CancellationToken, Task<IVectorAllocatorWrapperInternal<TVector>>> BuildAccumulator();
}
