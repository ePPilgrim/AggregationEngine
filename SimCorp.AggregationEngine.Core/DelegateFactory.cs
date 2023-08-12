using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;
using System.Data;

namespace SimCorp.AggregationEngine.Core;

internal class DelegateFactory<TVector, TResult> where TVector : struct, IAggregationPosition
{
    private readonly IAsyncDataAllocator<TVector> vectorCache;
    private readonly IAsyncDataAllocator<TResult> resultCache;
    private readonly Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator;
    private readonly Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator;

    public DelegateFactory(IAsyncDataAllocator<TVector> vectorCache, 
        IAsyncDataAllocator<TResult> resultCache,
        Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator,
        Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator)
    {
        this.vectorCache = vectorCache;
        this.resultCache = resultCache;
        this.calculator = calculator;
        this.accumulator = accumulator;
    }

    public async Task<TResult> Calculator(VectorWrapper<TVector> vector, IParameters parameter, CancellationToken token)
    {
        var stringKey = parameter.ToString();
        var timeStamp = await resultCache.GetTimeStampsAsync(new[] {stringKey}, token);
        if(timeStamp.First() > vector.TimeStamp)
        {
            return await resultCache.GetAsync(stringKey, token);
        }
        var vec = await vectorCache.GetAsync(vector.AllocatorKey, token);
        return await calculator(vec, parameter, token);
    }

    public async Task<VectorWrapper<TVector>> Accumulator(IEnumerable<VectorWrapper<TVector>> vectorWrappers, CancellationToken token)
    {
        int i = 0;
        int n = 2;
        List<TVector> subVectors = new();
        DateTime maxTime = vectorWrappers.Select(x => x.TimeStamp).Max();
        foreach( var vectorWrapper in vectorWrappers)
        {
            i++;
            var vec = await vectorWrapper.GetAsyncVector(token);
            subVectors.Add(vec);
            if( i == n )
            {
                var vector = await accumulator(subVectors, token);
                subVectors.Clear();
                subVectors.Add(vector);
                n = n + i - 1;
            }
        }
        if(subVectors.Count > 1 )
        {
            var vector = await accumulator(subVectors, token);
            subVectors.Clear();
            subVectors.Add(vector);
        }
        var wrappedVector = new VectorWrapper<TVector>(subVectors.First(), vectorCache);
        wrappedVector.TimeStamp = maxTime;

        return wrappedVector;
    }
}
