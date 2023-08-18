using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core;

public class DefaultAsyncAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> : IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult>
                                                                                                    where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                    where TUnorderedKey : IKey
                                                                                                    where TVector : IAggregationPosition
{
    private readonly IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, AllocatorWrapperInternal<TVector>, TResult> aggregationCalculationInternal;
    private readonly IAsyncExternalDataAllocator<TVector> vectorAllocator;
    private readonly IAsyncExternalDataAllocator<TResult> resultAllocator;
    private readonly IAsyncMapInternal<TUnorderedKey, TVector> leaves;
    private readonly IDataLayerFactory<TVector> vectorDataLayerFactory;
    private readonly IPositionDataLayerFactory<TVector> decoratedDataLayerFactory;
    private readonly IUnorderedKeyFactory<TUnorderedKey> unorderedKeyFactory;
    private readonly IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> aggregationCalculationFactory;

    private readonly Func<AllocatorWrapperInternal<TVector>, IParameters, CancellationToken, Task<TResult>> calculator;
    private readonly Func<IEnumerable<AllocatorWrapperInternal<TVector>>, CancellationToken, Task<AllocatorWrapperInternal<TVector>>> accumulator;
    private IAggregationStructure aggregationStructure;
    IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;
    IDataLayerFactory<TResult> resultDataLayerFactory;


    public DefaultAsyncAggregationCalculation(  IAsyncExternalDataAllocator<TResult> dataResultAllocator,
                                                IAsyncExternalDataAllocator<TVector> dataVectorAllocator,
                                                Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator
        )
    {
        this.dataResultAllocator = dataResultAllocator ?? throw new ArgumentNullException(nameof(dataResultAllocator));
        this.dataVectorAllocator = dataVectorAllocator ?? throw new ArgumentNullException(nameof(dataVectorAllocator));
        delegateFactory = new DelegateFactory<TVector, TResult>(dataVectorAllocator, dataResultAllocator, calculator, accumulator);
        this.calculator = delegateFactory.Calculator;
        this.accumulator = delegateFactory.Accumulator;

        aggregationCalculationInternal = new DefaultAsyncAggregationCalculationInternal<TOrderedKey, TUnorderedKey, AllocatorWrapperInternal<TVector>, TResult>(
            keyFactory, decoratedDataLayerFactory, resultDataLayerFactory
            );
    }
    public async Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, CancellationToken token)
    {
        using var delegates = aggregationCalculationFactory.CreateDelegateBuilder();
        var res = await aggregationCalculationInternal.AccumulateForSingleNodeAsync(nodeKey, delegates.Accumulator, token);
        return await res.GetVectorAsync(token);
    }

    public async Task<IDictionary<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        using var delegates = aggregationCalculationFactory.CreateDelegateBuilder();
        var res = await aggregationCalculationInternal.CalculateSingleNodeAsync(nodeKey, parameters, delegates.Calculator, delegates.Accumulator, token);
        return await res.GetAsync(parameters.Keys.ToHashSet(), token);
    }

    public async Task<IDictionary<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        using var delegates = aggregationCalculationFactory.CreateDelegateBuilder();
        var resInMap = await aggregationCalculationInternal.CalculateSubTreeAsync(rootNodeKey, parameters, delegates.Calculator, delegates.Accumulator, token);
        var res = await resInMap.GetAsync(resInMap.Keys.ToHashSet(), token);
        return res.ToDictionary(x => KeyValuePair.Create(x.Key.First, x.Key.Second), x => x.Value);
    }

    public IDictionary<TUnorderedKey, IMetaData> GetAllLeaves()
    {
        Dictionary<TUnorderedKey, IMetaData> res = new();
        foreach(var item in aggregationCalculationInternal.GetAllLeaves())
        {
            res[item.Key] = item.Value.Result.MetaData;
        }
        return res;
    }

    public async Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token)
    {
        await aggregationCalculationInternal.GetAllLeaves().RemoveAsync(keys, token);
        return await leaves.RemoveAsync(keys, token);
    }

    public async Task UpdateOrAddAsync(IEnumerable<TVector> positions, CancellationToken token)
    {
        var keyBuilder = unorderedKeyFactory.CreateUnorderedKeyBuilder();
        foreach(var position in positions)
        {
            await leaves.AddAsync(keyBuilder.BuildForVectors(position), position, token);
        }

        var leavesInteranl = aggregationCalculationInternal.GetAllLeaves();
        foreach (var key in leaves.Keys)
        {
            var isSuccess = await leavesInteranl.TryShallowInsert(key, token);
            if (isSuccess == false)
            {
                throw new KeyNotFoundException($"Inconsistent work of allocator layer. Or two different reference to the allocators");
            }
        }
    }
}
