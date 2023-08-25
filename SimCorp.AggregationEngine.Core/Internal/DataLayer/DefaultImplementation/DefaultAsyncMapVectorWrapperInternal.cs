using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;
using System.Collections;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;

internal class DefaultAsyncMapVectorWrapperInternal<TKey, TVector> : IAsyncMapVectorWrapperInternal<TKey, TVector> where TKey : IKey
                                                                                                                    where TVector : IAggregationPosition

{
    private readonly IAsyncMapInternal<TKey, TVector> internalAllocator;
    private readonly IDictionary<TKey, IVectorAllocatorWrapperInternal<TVector>> mapToVectors;
    private readonly IPositionDataLayerFactory<TVector> factory;

    public DefaultAsyncMapVectorWrapperInternal(IAsyncMapInternal<TKey, TVector> internalAllocator, IPositionDataLayerFactory<TVector> factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        this.internalAllocator = internalAllocator ?? throw new ArgumentNullException(nameof(internalAllocator));
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
        var isSuccess = value.GetExternalAllocatorID() == GetExternalAllocatorID();
        if (isSuccess)
        {
            isSuccess = await internalAllocator.TryShallowAddAsync(key, token);
        }
        if (isSuccess == false)
        {
            var wrappedVector = value.GetVector();
            await internalAllocator.AddAsync(key, wrappedVector, token);
        }
        mapToVectors[key] = value;
    }

    public Task<bool> TryShallowAddAsync(TKey key, CancellationToken token)
    {
        if (mapToVectors.ContainsKey(key)) return Task.FromResult(true);
        return Task.FromResult(false);
    }

    public async Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, IVectorAllocatorWrapperInternal<TVector>>> positions, CancellationToken token)
    {
        foreach (var position in positions)
        {
            await AddAsync(position.Key, position.Value, token);
        }
    }

    public async Task RemoveAsync(IEnumerable<TKey> keys, CancellationToken token)
    {
        await internalAllocator.RemoveAsync(keys, token);
        foreach (var key in keys.Where(x => mapToVectors.ContainsKey(x)).ToHashSet())
        {
            mapToVectors.Remove(key);
        }
    }

    public string GetExternalAllocatorID()
    {
        return internalAllocator.GetExternalAllocatorID();
    }

    public object Clone()
    {
        var newMap = factory.Create(internalAllocator);
        foreach (var item in mapToVectors)
        {
            newMap.AddAsync(item.Key, item.Value, CancellationToken.None).Wait();
        }
        return newMap;
    }

    public void Dispose()
    {
        internalAllocator.Dispose();
        mapToVectors.Clear();
    }

    public IEnumerator<KeyValuePair<TKey, Task<IVectorAllocatorWrapperInternal<TVector>>>> GetEnumerator()
    {
        foreach (var item in mapToVectors)
        {
            yield return KeyValuePair.Create(item.Key, Task.FromResult(item.Value));
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IVectorAllocatorWrapperInternal<TVector> buildAllocatorWrapperInternal(TKey key, TVector value)
    {
        return new AllocatorWrapperInternal(key, value, internalAllocator);
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

        public string GetExternalAllocatorID() => allocator.GetExternalAllocatorID();
        public DateTime TimeStamp { get; set; }
        public IMetaData MetaData => metaData;
        public IKey Key => key;

        //public int? PositionIK { get => MetaData.PositionIK; set => MetaData.PositionIK = value; }
        //public int? SecurityIK { get => MetaData.SecurityIK; set => MetaData.SecurityIK = value; }
        //public int? HoldingIK { get => MetaData.HoldingIK; set => MetaData.HoldingIK = value; }
        //public string? Currency { get => MetaData.Currency; set => MetaData.Currency = value; }
        //public string? Portfolio { get => MetaData.Portfolio; set => MetaData.Portfolio = value; }
        //public string? FreeCode1 { get => MetaData.FreeCode1; set => MetaData.FreeCode1 = value; }
        //public string? FreeCode2 { get => MetaData.FreeCode2; set => MetaData.FreeCode2 = value; }
        //public string? FreeCode3 { get; set; }
    }
}
