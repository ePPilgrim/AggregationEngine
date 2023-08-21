using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;

internal class DefaultDelegateBuilderInternal<TKey, TVector, TResult> : IDelegateBuilderInternal<TKey, TVector, TResult>    where TKey : IOrderedKey<TKey>
                                                                                                                            where TVector : IAggregationPosition
{
    private readonly Func<TVector, IParameters, CancellationToken, Task<TResult>> innerCalculator;
    private readonly Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> innerAccumulator;
    private readonly IAsyncMapVectorWrapperInternal<TKey, TVector> allocator;
    private readonly IPositionDataLayerFactory<TVector> dataLayerFactory;
    private readonly IOptimizationPolicyInternal optimizationPolicy;

    public DefaultDelegateBuilderInternal(  Func<TVector, IParameters, CancellationToken, Task<TResult>> innerCalculator, 
                                            Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> innerAccumulator,
                                            IPositionDataLayerFactory<TVector> dataLayerFactory,
                                            IOptimizationPolicyInternal optimizationPolicy)
    {
        this.innerCalculator = innerCalculator;
        this.innerAccumulator = innerAccumulator;
        this.dataLayerFactory = dataLayerFactory ?? throw new ArgumentNullException(nameof(dataLayerFactory));
        this.optimizationPolicy = optimizationPolicy ?? throw new ArgumentNullException(nameof(optimizationPolicy));
        allocator = dataLayerFactory.Create<TKey>(dataLayerFactory.Create<TKey>());
    }


    public Func<IVectorAllocatorWrapperInternal<TVector>, IParameters, CancellationToken, Task<TResult>> BuildCalculator()
    {
        return calculator;
    }

    public Func<IEnumerable<IVectorAllocatorWrapperInternal<TVector>>, TKey, CancellationToken, Task<IVectorAllocatorWrapperInternal<TVector>>> BuildAccumulator()
    {
        return accumulator;
    }

    public void Dispose()
    {
        allocator.Dispose();
    }

    private async Task<TResult> calculator(IVectorAllocatorWrapperInternal<TVector> vector, IParameters parameter, CancellationToken token)
    {
        var wrappedVector = await vector.GetVectorAsync(token);
        return await innerCalculator(wrappedVector, parameter, token);
    }

    private async Task<IVectorAllocatorWrapperInternal<TVector>> accumulator(IEnumerable<IVectorAllocatorWrapperInternal<TVector>> vectors, TKey key, CancellationToken token)
    {
        List<TVector> subVectors = new();
        int i = 0;
        int n = optimizationPolicy.VectorChankSize;
        foreach (var vector in vectors)
        {
            i++;
            var subVector  = await vector.GetVectorAsync(token);
            subVectors.Add(subVector);
            if (i == n)
            {
                var resultedWrappedVector = await innerAccumulator(subVectors, token);
                subVectors.Clear();
                subVectors.Add(resultedWrappedVector);
                n = optimizationPolicy.VectorChankSize + i - 1;
            }
        }
        if (subVectors.Count > 1)
        {
            var vector = await innerAccumulator(subVectors, token);
            subVectors.Clear();
            subVectors.Add(vector);
        }
        return await allocator.AddAsync(key, subVectors.First(), token);
    }
}
