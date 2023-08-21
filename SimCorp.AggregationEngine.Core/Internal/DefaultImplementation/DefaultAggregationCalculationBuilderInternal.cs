using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;

namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;
internal class DefaultAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, TVector, TResult> : IAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                                                                                                                    where TUnorderedKey : IKey
                                                                                                                                                                                                    where TVector : IMetaData
{
    private readonly IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;
    private readonly IDataLayerFactory<TResult> resultDataLayerFactory;

    public DefaultAggregationCalculationBuilderInternal(IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory, IDataLayerFactory<TResult> resultDataLayerFactory)
    {
        this.keyFactory = keyFactory;
        this.resultDataLayerFactory = resultDataLayerFactory;
    }

    public IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult> Build(IAsyncMapInternal<TUnorderedKey, TVector> leaves, IAggregationStructure aggregationStructure)
    {
        return new DefaultAsyncAggregationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult>(keyFactory, resultDataLayerFactory, leaves, aggregationStructure);
    }
}
