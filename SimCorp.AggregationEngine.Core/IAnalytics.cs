using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core;

public interface IAnalytics<TVector, TResult> : IAsyncAggrecationCalculation<OrderedKey, UnorderedKey, TVector, TResult>  where TVector : IAggregationPosition
{
}
