using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core;

public class DefaultAsyncAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> : IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult>
                                                                                                    where TOrderedKey : IKey
                                                                                                    where TUnorderedKey : IEqualityComparer<TUnorderedKey>
                                                                                                    where TVector : struct, IAggregationPosition
{
    private readonly IAsyncAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, VectorWrapper<TVector>, TResult> aggregationCalculationInternal;
    private readonly IAsyncDataAllocator<TVector> dataVectorAllocator;
    private readonly IAsyncDataAllocator<TResult> dataResultAllocator;
    private readonly Func<VectorWrapper<TVector>, IParameters, CancellationToken, Task<TResult>> calculator;
    private readonly Func<IEnumerable<VectorWrapper<TVector>>, CancellationToken, Task<VectorWrapper<TVector>>> accumulator;
    private readonly DelegateFactory<TVector, TResult> delegateFactory;
    private IAggregationStructure aggregationStructure;


    public DefaultAsyncAggregationCalculation(  IAsyncDataAllocator<TResult> dataResultAllocator,
                                                IAsyncDataAllocator<TVector> dataVectorAllocator,
                                                Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
                                                Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator
        )
    {
        this.dataResultAllocator = dataResultAllocator ?? throw new ArgumentNullException(nameof(dataResultAllocator));
        this.dataVectorAllocator = dataVectorAllocator ?? throw new ArgumentNullException(nameof(dataVectorAllocator));
        delegateFactory = new DelegateFactory<TVector, TResult>(dataVectorAllocator, dataResultAllocator, calculator, accumulator);
        this.calculator = delegateFactory.Calculator;
        this.accumulator = delegateFactory.Accumulator;
    }
    public async Task<TVector> AccumulateForSingleNodeAsync(TOrderedKey nodeKey, CancellationToken token)
    {
        var res = await aggregationCalculationInternal.AccumulateForSingleNodeAsync(nodeKey, accumulator, token);
        return await res.GetAsyncVector(token);
    }

    public async Task<IDictionary<TUnorderedKey, TResult>> CalculateSingleNodeAsync(TOrderedKey nodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        var res = await aggregationCalculationInternal.CalculateSingleNodeAsync(nodeKey, parameters, calculator, accumulator, token);
        return await res.GetAsync(parameters.Keys.ToHashSet(), token);
    }

    public async Task<IDictionary<KeyValuePair<TOrderedKey, TUnorderedKey>, TResult>> CalculateSubTreeAsync(TOrderedKey rootNodeKey, IDictionary<TUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        var res = await aggregationCalculationInternal.CalculateSubTreeAsync(rootNodeKey, parameters, calculator, accumulator, token);
        return await res.GetAsync(res.Keys.ToHashSet(), token); 
    }

    public async Task<IDictionary<TUnorderedKey, TVector>> GetAllLeaves(CancellationToken token)
    {
        var res = aggregationCalculationInternal.GetAllLeaves();
        //return await res.GetAsync(res.Keys.ToHashSet(), token);
    }

    public Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TUnorderedKey> keys, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrAddAsync(IEnumerable<TVector> positions, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
