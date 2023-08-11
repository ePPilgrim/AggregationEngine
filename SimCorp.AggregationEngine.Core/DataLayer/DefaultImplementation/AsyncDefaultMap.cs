namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class AsyncDefaultMap<TKey, TValue> : IAsyncMap<TKey, TValue>
{
    private readonly IDictionary<TKey, string> mapToAllocatorKey;
    private readonly IAsyncDataAllocator<TValue> dataAllocator;
    private readonly IKeyToStringConvertor<TKey> keyToString; //key helper

    public AsyncDefaultMap(IAsyncDataAllocator<TValue> dataAllocator, IKeyToStringConvertor<TKey> keyToString)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
        this.keyToString = keyToString ?? throw new ArgumentNullException(nameof(keyToString));
    }

    public IAsyncEnumerable<TValue> Values => dataAllocator.Get(mapToAllocatorKey.Values);

    public IEnumerable<TKey> Keys => mapToAllocatorKey.Keys;

    public async Task<bool> Contains(KeyValuePair<TKey, TValue> item)
    {
        if(!mapToAllocatorKey.ContainsKey(item.Key)) 
        { 
            return await Task.FromResult(false);
        }
        var allocItem = KeyValuePair.Create(mapToAllocatorKey[item.Key], item.Value);
        var res = await dataAllocator.Contains(new[]{allocItem});  
        return res.First();
    }

    public bool ContainsKey(TKey key)
    {
        return mapToAllocatorKey.ContainsKey(key);
    }

    public IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
       // return dataAllocator.Get(ma)
    }

    public Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TKey> keys, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
