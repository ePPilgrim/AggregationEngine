using SimCorp.AggregationEngine.Core.DataCollections;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;

internal class DefaultAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> : IAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult> 
                                                                                                                                where TOrderedKey : IKey
                                                                                                                                where TUnorderedKey : IEqualityComparer<TUnorderedKey>
                                                                                                                                where TVector : struct, IAggregationPosition
{
    private readonly IEmptyMapFactoryInternal<TOrderedKey,TUnorderedKey, TVector, TResult> emptyMapFactory;
    private readonly IOrderedKeyFactory<TOrderedKey, TVector> orderedKeyFactory;
    private readonly IMap<TUnorderedKey, TVector> leaves;
    private IAggregationStructure aggregationStructure;

    public DefaultAggregationCalculation(   IEmptyMapFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> emptyMapFactory,
                                            IOrderedKeyFactory<TOrderedKey, TVector> orderedKeyFactory,
                                            IAggregationStructure aggregationStructure)
    {
        this.emptyMapFactory = emptyMapFactory ?? throw new ArgumentNullException(nameof(emptyMapFactory));
        this.orderedKeyFactory = orderedKeyFactory ?? throw new ArgumentNullException(nameof(orderedKeyFactory));
        this.aggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof(aggregationStructure));
        leaves = emptyMapFactory.CreateEmptyUnorderedVectorMap();
    }

    public TVector AccumulateForSingleNode(TOrderedKey nodeKey, Func<IEnumerable<TVector>, TVector> accumulator)
    {
        var accumulatedVectors = leaves.Values.Where(x => nodeKey.IsPrefixOf(buildKey(x)));
        return accumulator(accumulatedVectors);
    }

    public void Add(IMap<TUnorderedKey,TVector> positions)
    {
        leaves.AddRange(positions);
    }

    public IMap<TUnorderedKey, TResult> CalculateSingleNode(TOrderedKey nodeKey, 
                                                            IMap<TUnorderedKey,IParameters> parameters, 
                                                            Func<TVector, IParameters, TResult> calculator, 
                                                            Func<IEnumerable<TVector>, TVector> accumulator)
    {
        var res = emptyMapFactory.CreateEmptyUnorderedResultMap();
        var accumulatedVector = accumulator(leaves.Values.Where(x => nodeKey.IsPrefixOf(buildKey(x))));
        foreach(var val in parameters)
        {
            res[val.Key] = calculator(accumulatedVector, val.Value);
        }

        return res;
    }

    public IMap<TOrderedKey, IMap<TUnorderedKey, TResult>> CalculateSubTree(TOrderedKey rootNodeKey,
                                                                            IMap<TUnorderedKey,IParameters> parameters,
                                                                            Func<TVector, IParameters, TResult> calculator,
                                                                            Func<IEnumerable<TVector>, TVector> accumulator)
    {
        var keyOperations = orderedKeyFactory.CreateKeyOperations();
        var res = emptyMapFactory.CreateEmptyOrderedUnorderedResultMap();
        var levelNodeVectors = leaves.Values.Select(x => new { Key = buildKey(x), Value = x}).Where(x => rootNodeKey.IsPrefixOf(x.Key)); 

        foreach (var structure in aggregationStructure)
        {
            levelNodeVectors = levelNodeVectors.GroupBy(x => keyOperations.SubKey(x.Key, structure))
                                               .Select(x => new { x.Key, Value = accumulator(x.Select(y=>y.Value))});
            res.AddRange(levelNodeVectors.Select(x => KeyValuePair.Create(x.Key, calculateWithParameters(x.Value, parameters, calculator))));
            if (structure == rootNodeKey.AggregationStructure)
            {
                break;
            }
        }
        return res;
    }

    public void Clear()
    {
        leaves.Clear();
    }

    public IMap<TUnorderedKey, TVector> GetAllLeaves()
    {
        return leaves;
    }

    public void Remove(IEnumerable<TUnorderedKey> keys)
    {
        leaves.Remove(keys);
    }

    private IMap<TUnorderedKey, TResult> calculateWithParameters(TVector vector,
                                                              IMap<TUnorderedKey, IParameters> parameters,
                                                              Func<TVector, IParameters, TResult> calculator)
    {
        var res = emptyMapFactory.CreateEmptyUnorderedResultMap();
        var calculatedSequence = parameters.Select(x => KeyValuePair.Create(x.Key, calculator(vector, x.Value)));
        res.AddRange(calculatedSequence);
        return res;
    }

    private TOrderedKey buildKey(TVector vector) => orderedKeyFactory.CreateKeyBuilder(vector).Add(aggregationStructure).Build();
}
