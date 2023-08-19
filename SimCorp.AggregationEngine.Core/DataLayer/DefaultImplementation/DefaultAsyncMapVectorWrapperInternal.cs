using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using System.Collections;

namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class DefaultAsyncMapVectorWrapperInternal<TKey, TVector> : IAsyncMapVectorWrapperInternal<TKey, TVector>  where TKey : IKey
                                                                                                                    where TVector : IAggregationPosition
    
{
    private readonly IAsyncMapInternal<TKey, TVector> internalAllocator;
    private readonly IAsyncExternalDataAllocator<TVector> dataAllocator;
    private readonly IDictionary<TKey, IVectorAllocatorWrapperInternal<TVector>> mapToVectors;
    private readonly IPositionDataLayerFactory<TVector> factory;

    public DefaultAsyncMapVectorWrapperInternal(IAsyncExternalDataAllocator<TVector> dataAllocator, IPositionDataLayerFactory<TVector> factory)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        internalAllocator = factory.Create<TKey>();
        mapToVectors = new Dictionary<TKey, IVectorAllocatorWrapperInternal<TVector>>();
    }

    public ICollection<TKey> Keys => internalAllocator.Keys;

    public IEnumerable<IVectorAllocatorWrapperInternal<TVector>> GetAllValues()
    {
        return mapToVectors.Values;
    }

    public Task<IVectorAllocatorWrapperInternal<TVector>> GetAsync(TKey key, CancellationToken token)
    {
        if (!mapToVectors.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Required key {key} is not found.");
        }
        return Task.FromResult(mapToVectors[key]);
    }

    public Task<IDictionary<TKey, IVectorAllocatorWrapperInternal<TVector>>> GetAsync(HashSet<TKey> keys, CancellationToken token)
    {
        if (!keys.All(x => mapToVectors.ContainsKey(x)))
        {
            throw new KeyNotFoundException($"Not all required keys are found.");
        }
        var res = mapToVectors.Where(x => keys.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        return Task.FromResult((IDictionary<TKey, IVectorAllocatorWrapperInternal<TVector>>)res);
    }

    public async Task<IVectorAllocatorWrapperInternal<TVector>> AddAsync(TKey key, TVector value, CancellationToken token)
    {
        await internalAllocator.AddAsync(key, value, token);
        mapToVectors[key] = buildAllocatorWrapperInternal(key, value);
        return mapToVectors[key];
    }

    public async Task AddAsync(TKey key, IVectorAllocatorWrapperInternal<TVector> value, CancellationToken token)
    {
        var isSuccess = await TryShallowAddAsync(key, token);
        if(isSuccess == false)
        {
            await UpdateOrAddAsync(new[] {KeyValuePair.Create(key, value)}, token);   
        }
    }

    public Task<bool> TryShallowAddAsync(TKey key, CancellationToken token)
    {
        if (mapToVectors.ContainsKey(key)) return Task.FromResult(true);
        return Task.FromResult(false);
    }

    public async Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, IVectorAllocatorWrapperInternal<TVector>>> positions, CancellationToken token)
    {
        //foreach()

        //await internalAllocator.UpdateOrAddAsync(positions.Select(x => KeyValuePair.Create(x.Key, )))
    }

    public async Task RemoveAsync(IEnumerable<TKey> keys, CancellationToken token)
    {
        await internalAllocator.RemoveAsync(keys, token);
        foreach (var key in keys.Where(x => mapToVectors.ContainsKey(x)).ToHashSet())
        {
            mapToVectors.Remove(key);
        }
    }

    public object Clone()
    {
        var newMap = factory.Create(internalAllocator);
        
    }

    public void Dispose()
    {
        internalAllocator.Dispose();
        mapToVectors.Clear();
    }

    public IEnumerator<KeyValuePair<TKey, Task<IVectorAllocatorWrapperInternal<TVector>>>> GetEnumerator()
    {
        throw new NotImplementedException();
    }




    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
    
    private IVectorAllocatorWrapperInternal<TVector> buildAllocatorWrapperInternal(TKey key, TVector value)
    {
        return new AllocatorWrapperInternal(key, value.MetaData, internalAllocator);
    }
    private class AllocatorWrapperInternal : IVectorAllocatorWrapperInternal<TVector>
    {
        private readonly IMetaData metaData;
        private readonly IAsyncMapInternal<TKey, TVector> allocator;
        private readonly TKey key;

        public AllocatorWrapperInternal(TKey key, IMetaData metaData, IAsyncMapInternal<TKey, TVector> allocator)
        {
            this.metaData = metaData;
            this.allocator = allocator;
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public async Task<TVector> GetVectorAsync(CancellationToken token)
        {
            return await allocator.GetAsync(key, token);
        }

        public TVector GetVector()
        {
            var res = allocator.GetAsync(key, CancellationToken.None);
            return res.Result;
        }

        public DateTime TimeStamp { get; set; }

        public IMetaData MetaData => metaData;
        public IKey Key => key;
    }
}
