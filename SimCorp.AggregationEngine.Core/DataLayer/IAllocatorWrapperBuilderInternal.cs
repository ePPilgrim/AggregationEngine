using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAllocatorWrapperBuilderInternal<TKey, TVector>  where TKey : IKey
                                                                    where TVector : IAggregationPosition
{
    AllocatorWrapperInternal<TVector> Build(TKey key, TVector position);
}
