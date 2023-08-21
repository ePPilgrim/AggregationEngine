using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                        where TUnorderedKey : IKey
                                                                                                        where TVector : IMetaData
{
    IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult> Build(IAsyncMapInternal<TUnorderedKey, TVector> leaves, IAggregationStructure aggregationStructure);
}
