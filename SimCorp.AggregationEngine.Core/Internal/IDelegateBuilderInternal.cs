using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IDelegateBuilderInternal<TKey, TVector, TResult> : IDisposable   where TKey : IOrderedKey<TKey>
                                                                                    where TVector : IAggregationPosition
{
    Func<AllocatorWrapperInternal<TVector>, IParameters, CancellationToken, Task<TResult>> Calculator { get; }
    Func<IEnumerable<AllocatorWrapperInternal<TVector>>, TKey, CancellationToken, Task<AllocatorWrapperInternal<TVector>>> Accumulator { get; }
}
