using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using System.Collections.Concurrent;

namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;

internal class DefaultAsyncAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> : IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult>
                                                                                                                                where TOrderedKey : IKey
                                                                                                                                where TUnorderedKey : IEqualityComparer<TUnorderedKey>, IEquatable<TUnorderedKey>
                                                                                                                                where TVector : struct, IAggregationPosition
{
    private readonly IEmptyAsyncMapFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> emptyMapFactory;
    private readonly IOrderedKeyFactory<TOrderedKey, TVector> orderedKeyFactory;
    private readonly IAsyncMap<TUnorderedKey, TVector> leaves;
    private IAggregationStructure aggregationStructure;

    public DefaultAsyncAggregationCalculation(IEmptyAsyncMapFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> emptyMapFactory,
                                            IOrderedKeyFactory<TOrderedKey, TVector> orderedKeyFactory,
                                            IAggregationStructure aggregationStructure)
    {
        this.emptyMapFactory = emptyMapFactory ?? throw new ArgumentNullException(nameof(emptyMapFactory));
        this.orderedKeyFactory = orderedKeyFactory ?? throw new ArgumentNullException(nameof(orderedKeyFactory));
        this.aggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof(aggregationStructure));
        leaves = emptyMapFactory.CreateEmptyUnorderedVectorAsyncMap();
    }

    public async Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, 
                                                            Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator, 
                                                            CancellationToken token)
    {
        var accumulatedVectors = leaves.Values.Where(x => nodeKey.IsPrefixOf(BuildKeyInternal(x)));
        return await accumulator(accumulatedVectors, token);
    }

    public async Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TUnorderedKey, TVector>> positions, CancellationToken token)
    {
        await leaves.UpdateOrAddAsync(positions, token);    
    }

    public async Task<IAsyncMap<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey,
                                                     IDictionary<TUnorderedKey, IParameters> parameters,
                                                     Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                     Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator,
                                                     CancellationToken token)
    {
        var accumulatedVector = await AccumulateForSingleNodeAsync(nodeKey, accumulator, token);
        ConcurrentDictionary<TUnorderedKey,Task<TResult>> resultsAsync = new();
        foreach (var item in parameters)
        {
            resultsAsync[item.Key] = calculator(accumulatedVector, item.Value, token);      
        }
        await Task.WhenAll(resultsAsync.Select(x => x.Value));
        var res = emptyMapFactory.CreateEmptyUnorderedResultAsyncMap();
        await res.UpdateOrAddAsync(resultsAsync.Select(x => KeyValuePair.Create(x.Key,x.Value.Result)), token);
        return res;
    }

    public async Task<IAsyncMap<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey,
                                                                    IDictionary<TUnorderedKey, IParameters> parameters,
                                                                    Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                                    Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator,
                                                                    CancellationToken token)
    {
        var res = emptyMapFactory.CreateEmptyOrderedUnorderedResultAsyncMap();
        var levelNodeVectors = leaves.Values.Select(x => KeyValuePair.Create(BuildKeyInternal(x), x)).Where(x => rootNodeKey.IsPrefixOf(x.Key));
        using (var tempAllocator = emptyMapFactory.CreateEmptyOrderedVectorAsyncMap()) {

            var keyOperations = orderedKeyFactory.CreateKeyOperations();
            foreach (var structure in aggregationStructure)
            {
                List<KeyValuePair<TOrderedKey, TVector>> tempLevelNodeVectors = new();
                foreach (var grp in levelNodeVectors.GroupBy(x => keyOperations.SubKey(x.Key, structure)))
                {
                    var accumulatedVector = await accumulator(grp.Select(x => x.Value), token);
                    tempLevelNodeVectors.Add(KeyValuePair.Create(grp.Key, accumulatedVector));
                }

                foreach (var node in tempLevelNodeVectors)
                {
                    ConcurrentDictionary<KeyValuePair<TOrderedKey, TUnorderedKey>, Task<TResult>> resultAsync = new();
                    foreach (var item in parameters)
                    {
                        var key = KeyValuePair.Create(node.Key, item.Key);
                        resultAsync[key] = calculator(node.Value, item.Value, token);
                    }
                    await Task.WhenAll(resultAsync.Values);
                    await res.UpdateOrAddAsync(resultAsync.Select(x => KeyValuePair.Create(x.Key, x.Value.Result)), token);
                }
                await tempAllocator.UpdateOrAddAsync(tempLevelNodeVectors, token);

                if (structure == rootNodeKey.AggregationStructure)
                {
                    break;
                }
                levelNodeVectors = tempLevelNodeVectors;
            }
        }
        return res;
    }

    public IAsyncMap<TUnorderedKey, TVector> GetAllLeaves()
    {
        return leaves;
    }

    public async Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token)
    {
        return await leaves.RemoveAsync(keys.ToArray(), token);
    }

    private TOrderedKey BuildKeyInternal(TVector vector) => orderedKeyFactory.CreateKeyBuilder(vector).Add(aggregationStructure).Build();

}

