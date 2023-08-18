using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using System.Collections;

namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class DefaultAsyncMapVectorDecoratorInternal<TKey, TVector> : IAsyncMapVectorDecoratorInternal<TKey, TVector> where TKey : IKey
                                                                                                                where TVector : IAggregationPosition
{
    private readonly IAsyncMapInternal<TKey, TVector> internalAllocator;
    private readonly IAsyncExternalDataAllocator<TVector> dataAllocator;
    private readonly IDictionary<TKey, AllocatorWrapperInternal<TVector>> mapToVectors;
    private readonly IPositionDataLayerFactory<TVector> factory;
    private readonly IAllocatorWrapperBuilderInternal<TKey,TVector> wrapperVectorBuilder;

    public DefaultAsyncMapVectorDecoratorInternal(IAsyncExternalDataAllocator<TVector> dataAllocator, IPositionDataLayerFactory<TVector> factory)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        internalAllocator = factory.Create<TKey>();
        wrapperVectorBuilder = factory.CreateBuilder(internalAllocator);
        mapToVectors = new Dictionary<TKey, AllocatorWrapperInternal<TVector>>();
    }

    public ICollection<TKey> Keys => internalAllocator.Keys;

    public int Count => internalAllocator.Count;

    public async Task AddAsync(TKey key, TVector value, CancellationToken token)
    {
        await AddAsync(key, wrapperVectorBuilder.Build(key, value), token );
    }

    public async Task AddAsync(TKey key, AllocatorWrapperInternal<TVector> value, CancellationToken token)
    {
        var isSuccess = await TryShallowInsert(key, token);
        if(isSuccess == false)
        {
            await UpdateOrAddAsync(new[] {KeyValuePair.Create(key, value)}, token);   
        }
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public Task<bool> ContainsAsync(KeyValuePair<TKey, AllocatorWrapperInternal<TVector>> item, CancellationToken token)
    {
        return Task.FromResult(mapToVectors.Contains(item));
    }

    public Task<IList<bool>> ContainsAsync(IEnumerable<KeyValuePair<TKey, AllocatorWrapperInternal<TVector>>> items, CancellationToken token)
    {
        var res = items.Select(x => mapToVectors.Contains(x)).ToList();
        return Task.FromResult((IList<bool>)res);
    }

    public bool ContainsKey(TKey key)
    {
        return mapToVectors.ContainsKey(key);
    }

    public void Dispose()
    {
        internalAllocator.Dispose();
    }

    public IEnumerable<AllocatorWrapperInternal<TVector>> GetAllValues()
    {
        return mapToVectors.Values;
    }

    public IAsyncEnumerable<AllocatorWrapperInternal<TVector>> GetAllValuesAsync()
    {
        return mapToVectors.Values.ToAsyncEnumerable();
    }

    public Task<IDictionary<TKey, AllocatorWrapperInternal<TVector>>> GetAsync(HashSet<TKey> keys, CancellationToken token)
    {
        if (!keys.All(x => mapToVectors.ContainsKey(x)))
        {
            throw new KeyNotFoundException($"Not all required keys are found.");
        }
        var res = mapToVectors.Where(x => keys.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        return Task.FromResult((IDictionary<TKey, AllocatorWrapperInternal<TVector>>)res);
    }

    public Task<AllocatorWrapperInternal<TVector>> GetAsync(TKey key, CancellationToken token)
    {
        if (!mapToVectors.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Required key {key} is not found.");
        }
        return Task.FromResult(mapToVectors[key]);
    }

    public IAsyncEnumerator<KeyValuePair<TKey, AllocatorWrapperInternal<TVector>>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, Task<AllocatorWrapperInternal<TVector>>>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TKey> keys, CancellationToken token)
    {
        return internalAllocator.RemoveAsync(keys, token);
    }

    public async Task TouchAsync(TKey key, CancellationToken token)
    {
        await internalAllocator.TouchAsync(key, token);   
    }

    public async Task TouchAsync(HashSet<TKey> keys, CancellationToken token)
    {
        await internalAllocator.TouchAsync(keys, token);
    }

    public async Task<bool> TryShallowInsert(TKey key, CancellationToken token)
    {
        if (mapToVectors.ContainsKey(key)) return true;
        return await internalAllocator.TryShallowInsert(key, token);
    }

    public Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, AllocatorWrapperInternal<TVector>>> positions, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    Task<AllocatorWrapperInternal<TVector>> IAsyncMapVectorDecoratorInternal<TKey, TVector>.AddAsync(TKey key, TVector wrappedPosition, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
