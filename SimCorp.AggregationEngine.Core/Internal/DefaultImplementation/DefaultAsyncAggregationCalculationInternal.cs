using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.AggregationStructure;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;
using System.Collections.Concurrent;

namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;

internal class DefaultAsyncAggregationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult> : IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult>
                                                                                                                                where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                                                where TUnorderedKey : IKey
                                                                                                                                where TVector : IAggregationPosition
{
    private readonly IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;
    private readonly IDataLayerFactory<TVector> vectorDataLayerFactory;
    private readonly IDataLayerFactory<TResult> resultDataLayerFactory;
    private readonly IAsyncMapInternal<TUnorderedKey, TVector> leaves;
    private readonly IUnorderedKeyBuilder<TUnorderedKey> unorderedKeyBuilder;
    private IAggregationStructure aggregationStructure;
    private IOrderedKeyBuilder<TOrderedKey> orderedKeyBuilder;

    public DefaultAsyncAggregationCalculationInternal(  IKeyFactory<TOrderedKey,TUnorderedKey> keyFactory,
                                                        IDataLayerFactory<TVector> vectorDataLayerFactory,
                                                        IDataLayerFactory<TResult> resultDataLayerFactory)
    {
        this.keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        this.vectorDataLayerFactory = vectorDataLayerFactory ?? throw new ArgumentNullException(nameof(vectorDataLayerFactory));
        this.resultDataLayerFactory = resultDataLayerFactory ?? throw new ArgumentNullException(nameof(resultDataLayerFactory));

        leaves = vectorDataLayerFactory.Create<TUnorderedKey>();
        aggregationStructure = keyFactory.CreateAggregationStructureBuilder().BuildEmptyAggregationStructure();
        unorderedKeyBuilder = keyFactory.CreateUnorderedKeyBuilder();
        orderedKeyBuilder = keyFactory.CreateOrderedKeyBuilder(aggregationStructure);
    }

    public IAggregationStructure AggregartionStructure { 
        get => aggregationStructure; 
        set
        {
            aggregationStructure = value;
            orderedKeyBuilder = keyFactory.CreateOrderedKeyBuilder(value);
        }
    }

    public async Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, 
                                                            Func<IEnumerable<TVector>, TOrderedKey, CancellationToken, Task<TVector>> accumulator, 
                                                            CancellationToken token)
    {
        var accumulatedVectors = leaves.GetAllValues().Where(x => nodeKey.IsPrefixOf(orderedKeyBuilder.Build(x)));
        return await accumulator(accumulatedVectors, nodeKey, token);
    }

    public async Task<IAsyncMapInternal<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey,
                                                     IDictionary<TUnorderedKey, IParameters> parameters,
                                                     Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                     Func<IEnumerable<TVector>, TOrderedKey, CancellationToken, Task<TVector>> accumulator,
                                                     CancellationToken token)
    {
        var accumulatedVector = await AccumulateForSingleNodeAsync(nodeKey, accumulator, token);
        ConcurrentDictionary<TUnorderedKey,Task<TResult>> resultsAsync = new();
        foreach (var item in parameters)
        {
            resultsAsync[item.Key] = calculator(accumulatedVector, item.Value, token);      
        }
        await Task.WhenAll(resultsAsync.Select(x => x.Value));
        var res = resultDataLayerFactory.Create<TUnorderedKey>();
        await res.UpdateOrAddAsync(resultsAsync.Select(x => KeyValuePair.Create(x.Key,x.Value.Result)), token);
        return res;
    }

    public async Task<IAsyncMapInternal<DualKey<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey,
                                                                    IDictionary<TUnorderedKey, IParameters> parameters,
                                                                    Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                    Func<IEnumerable<TVector>, TOrderedKey, CancellationToken, Task<TVector>> accumulator,
                                                                    CancellationToken token)
    {
        var res = resultDataLayerFactory.Create<DualKey<TOrderedKey, TUnorderedKey>>();
        using var tempLeaves = (IAsyncMapInternal<TUnorderedKey,TVector>)leaves.Clone();

        var levelNodeVectors = tempLeaves.GetAllValues().Select(x => KeyValuePair.Create(orderedKeyBuilder.Build(x),x)).Where(x => rootNodeKey.IsPrefixOf(x.Key));

        foreach (var level in aggregationStructure)
        {
            var subStructure = aggregationStructure.GetSubStructureAt(level);
            List<KeyValuePair<TOrderedKey, TVector>> tempLevelNodeVectors = new();
            foreach (var grp in levelNodeVectors.GroupBy(x => x.Key.GetSubKey(subStructure)))
            {
                var accumulatedVector = await accumulator(grp.Select(x => x.Value), grp.Key , token);
                tempLevelNodeVectors.Add(KeyValuePair.Create(grp.Key, accumulatedVector));
            }

            foreach (var node in tempLevelNodeVectors)
            {
                ConcurrentDictionary<DualKey<TOrderedKey, TUnorderedKey>, Task<TResult>> resultAsync = new();
                foreach (var item in parameters)
                {
                    var key = new DualKey<TOrderedKey,TUnorderedKey>(node.Key, item.Key);
                    resultAsync[key] = calculator(node.Value, item.Value, token);
                }
                await Task.WhenAll(resultAsync.Values);
                await res.UpdateOrAddAsync(resultAsync.Select(x => KeyValuePair.Create(x.Key, x.Value.Result)), token);
            }

            if (rootNodeKey.GetSubKey(subStructure).IsEmpty()) continue;
            levelNodeVectors = tempLevelNodeVectors;
        }
        return res;
    }

    public IAsyncMapInternal<TUnorderedKey, TVector> GetAllLeaves()
    {
        return leaves;
    }
}

