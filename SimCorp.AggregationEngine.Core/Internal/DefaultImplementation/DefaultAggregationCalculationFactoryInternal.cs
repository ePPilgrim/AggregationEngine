using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;

internal class DefaultAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> : IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>
where TOrderedKey : IOrderedKey<TOrderedKey>
where TUnorderedKey : IKey
where TVector : IAggregationPosition
{
    private readonly IPositionDataLayerFactory<TVector> positionDataLayerFactory;
    private readonly IDataLayerFactory<TResult> resultDataLayerFactory;
    private readonly IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;
    private readonly IOptimizationPolicyInternal optimizationPolicy;

    public DefaultAggregationCalculationFactoryInternal(IPositionDataLayerFactory<TVector> positionDataLayerFactory,
                                                        IDataLayerFactory<TResult> resultDataLayerFactory,
                                                        IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory,
                                                        IOptimizationPolicyInternal optimizationPolicy)
    {
        this.positionDataLayerFactory = positionDataLayerFactory ?? throw new ArgumentNullException(nameof(positionDataLayerFactory));
        this.resultDataLayerFactory = resultDataLayerFactory ?? throw new ArgumentNullException(nameof(resultDataLayerFactory));
        this.keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        this.optimizationPolicy = optimizationPolicy ?? throw new ArgumentNullException(nameof(optimizationPolicy));
    }

    public IAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, IVectorAllocatorWrapperInternal<TVector>, TResult> CreateAggregationCalculationBuilder()
    {
        return new DefaultAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, IVectorAllocatorWrapperInternal<TVector>, TResult>(keyFactory, resultDataLayerFactory);
    }

    public IDelegateBuilderInternal<TOrderedKey, TVector, TResult> CreateDelegateBuilder(Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                                         Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator)
    {
        return new DefaultDelegateBuilderInternal<TOrderedKey, TVector, TResult>(calculator, accumulator,positionDataLayerFactory, optimizationPolicy);
    }

    public IAsyncMapInternal<TKey, TVector> CreatePositionAllocator<TKey>() where TKey : IKey
    {
        return positionDataLayerFactory.Create<TKey>();  
    }

    public IAsyncMapInternal<TUnorderedKey, TResult> CreateResultAllocator()
    {
       return resultDataLayerFactory.Create<TUnorderedKey>();
    }

    public IAsyncMapVectorWrapperInternal<TKey, TVector> CreateWrapperPositionAllocator<TKey>() where TKey : IKey
    {
        return positionDataLayerFactory.Create<TKey>(positionDataLayerFactory.Create<TKey>());
    }
}
