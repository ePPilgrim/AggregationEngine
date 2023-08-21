using Microsoft.Extensions.DependencyInjection;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core;

public class DefaultAsyncAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> : IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult>
                                                                                                    where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                    where TUnorderedKey : IKey
                                                                                                    where TVector : IAggregationPosition
{
    private readonly IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> internalsFactory;
    private readonly IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;
    private readonly Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator;
    private readonly Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator;

    private readonly IAsyncMapVectorWrapperInternal<TUnorderedKey, TVector> leaves;
    private readonly IAggregationCalculationBuilderInternal<TOrderedKey, TUnorderedKey, IVectorAllocatorWrapperInternal<TVector>, TResult> aggregationCalculationInternalBuilder;
    private readonly IAggregationStructureBuilder aggregationStructureBuilder;

    private IAggregationStructure aggregationStructure;
    private IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, IVectorAllocatorWrapperInternal<TVector>, TResult> aggregationCalculationInternal;


    public DefaultAsyncAggregationCalculation(  IServiceProvider internalServicesProvider,
                                                IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory,
                                                Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator)
    {
        internalsFactory = internalServicesProvider.GetRequiredService<IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>>();
        if(internalsFactory == null) throw new ArgumentNullException(nameof(internalsFactory));
        this.keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        this.calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        this.accumulator = accumulator ?? throw new ArgumentNullException(nameof(accumulator));

        leaves = internalsFactory.CreateWrapperPositionAllocator<TUnorderedKey>();
        aggregationStructureBuilder = keyFactory.CreateAggregationStructureBuilder();
        aggregationCalculationInternalBuilder = internalsFactory.CreateAggregationCalculationBuilder();

        aggregationStructure = aggregationStructureBuilder.BuildEmptyAggregationStructure();
        aggregationCalculationInternal = aggregationCalculationInternalBuilder.Build(leaves, aggregationStructure); 
    }

    public void SetAggregationStructure(IEnumerable<AggregationLevel> aggregationStructureDesign)
    {
        aggregationStructure = aggregationStructureBuilder.BuildFromSequence(aggregationStructureDesign);
        aggregationCalculationInternal = aggregationCalculationInternalBuilder.Build(leaves, aggregationStructure);
    }

    public async Task UpdateOrAddAsync(IEnumerable<TVector> positions, CancellationToken token)
    {
        var keyBuilder = keyFactory.CreateUnorderedKeyBuilder();
        foreach (var position in positions)
        {
            await leaves.AddAsync(keyBuilder.BuildForPositions(position), position, token);
        }
    }

    public IDictionary<TUnorderedKey, IMetaData> GetAllLeaves()
    {
        Dictionary<TUnorderedKey, IMetaData> res = new();
        foreach (var item in leaves)
        {
            res[item.Key] = item.Value.Result.MetaData;
        }
        return res;
    }

    public async Task RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token)
    { 
        await leaves.RemoveAsync(keys, token);
    }

    public async Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, CancellationToken token)
    {
        using var delegateBuilder = internalsFactory.CreateDelegateBuilder(calculator, accumulator);
        var res = await aggregationCalculationInternal.AccumulateForSingleNodeAsync(nodeKey, delegateBuilder.BuildAccumulator(), token);
        return await res.GetVectorAsync(token);
    }

    public async Task<IDictionary<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        using var delegateBuilder = internalsFactory.CreateDelegateBuilder(calculator, accumulator);
        var res = await aggregationCalculationInternal.CalculateSingleNodeAsync(nodeKey, parameters, delegateBuilder.BuildCalculator(), delegateBuilder.BuildAccumulator(), token);
        return await res.GetAsync(parameters.Keys.ToHashSet(), token);
    }

    public async Task<IDictionary<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        using var delegateBuilder = internalsFactory.CreateDelegateBuilder(calculator, accumulator);
        var resInMap = await aggregationCalculationInternal.CalculateSubTreeAsync(rootNodeKey, parameters, delegateBuilder.BuildCalculator(), delegateBuilder.BuildAccumulator(), token);
        var res = await resInMap.GetAsync(resInMap.Keys.ToHashSet(), token);
        return res.ToDictionary(x => KeyValuePair.Create(x.Key.First, x.Key.Second), x => x.Value);
    }
}
