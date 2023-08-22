using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core;

public interface IAggregationCalculationBuilder<TOrderedKey, TUnorderedKey, TVector, TResult>   where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                where TUnorderedKey : IKey
                                                                                                where TVector : IAggregationPosition
{
    IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> Build(Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                                        Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator);
}
