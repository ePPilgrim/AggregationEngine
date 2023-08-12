namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class AsyncDefaultMap<TKey, TValue> : IAsyncMap<TKey, TValue> where TKey : notnull, IEquatable<TKey>
{
    private readonly IDictionary<TKey, string> mapToAllocatorKey;
    private readonly IAsyncDataAllocator<TValue> dataAllocator;
    private readonly IKeyToStringConvertor<TKey> keyToString; //key helper

    public AsyncDefaultMap(IAsyncDataAllocator<TValue> dataAllocator, IKeyToStringConvertor<TKey> keyToString)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
        this.keyToString = keyToString ?? throw new ArgumentNullException(nameof(keyToString));
        mapToAllocatorKey = new Dictionary<TKey, string>();
    }

    public IAsyncEnumerable<TValue> Values => dataAllocator.FetchAsAsyncSequence(mapToAllocatorKey.Values);

    public ICollection<TKey> Keys => mapToAllocatorKey.Keys;

    public int Count => Keys.Count;

    public async Task<IDictionary<TKey,TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token)
    {
        if(!keys.All(x => mapToAllocatorKey.ContainsKey(x)))
        {
            throw new KeyNotFoundException($"Not all required keys are found.");
        }
        var allocKeys = keys.Select(x => mapToAllocatorKey[x]).ToHashSet();
        var fetchedValues = await dataAllocator.FetchAtOnceAsync(allocKeys, token);
        return keys.ToDictionary(x => x, x => fetchedValues[mapToAllocatorKey[x]]);
    }

    public async Task<TValue> GetAsync(TKey key, CancellationToken token)
    {
        if (!mapToAllocatorKey.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Required key {key} is not found.");
        }

        return await dataAllocator.GetAsync(mapToAllocatorKey[key], token);
    }

    public async Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token)
    {
        var allocEnumerator = positions.Select(x => KeyValuePair.Create(keyToString.KeyToString(x.Key), x.Value));
        await dataAllocator.SetAsync(allocEnumerator, token);
        var keyTokeyMapping = positions.Where(x => !mapToAllocatorKey.ContainsKey(x.Key)).Select(x => KeyValuePair.Create(x.Key, keyToString.KeyToString(x.Key)));
        foreach(var item in keyTokeyMapping.Distinct())
        {
            mapToAllocatorKey.Add(item.Key, item.Value);
        }
    }

    public async Task<IList<bool>> RemoveAsync(IList<TKey> keys, CancellationToken token)
    {
        var keyToStringKeyItems = keys.Where(x => mapToAllocatorKey.ContainsKey(x)).Select(x => new { Key = x, AllocKey = mapToAllocatorKey[x] }).ToList();
        var whatRemoved = await dataAllocator.RemoveAsync(keyToStringKeyItems.Select(x => x.AllocKey), token);
        var keyValMap = keyToStringKeyItems.Zip(whatRemoved).GroupBy(x => x.First.Key).ToDictionary(x => x.Key, x => x.First().Second);

        List<bool> res = new();
        foreach(var key in keys)
        {
            bool val;
            if (keyValMap.TryGetValue(key, out val))
            {
                res.Add(val);
            }
            else
            {
                res.Add(false);
            }
        }
        return res;
    }

    public async Task<bool> ContainsAsync(KeyValuePair<TKey, TValue> item, CancellationToken token)
    {
        string allocKey;
        if(!mapToAllocatorKey.TryGetValue(item.Key, out allocKey))
        {
            return false;
        }
        var res = await dataAllocator.ContainsAsync(new[] { KeyValuePair.Create(mapToAllocatorKey[item.Key], item.Value)}, token);
        return res.First();
    }

    public async Task<IList<bool>> ContainsAsync(IEnumerable<KeyValuePair<TKey, TValue>> items, CancellationToken token)
    {
        var expanded = items.Where(x => mapToAllocatorKey.ContainsKey(x.Key))
            .Select(x => new { x.Key, Value = KeyValuePair.Create(mapToAllocatorKey[x.Key], x.Value) });
        var whatContains = await dataAllocator.ContainsAsync(expanded.Select(x => x.Value), token); 
        var keyValMap = expanded.Select(x => x.Key).Zip(whatContains).GroupBy(x => x.First).ToDictionary(x => x.Key, x => x.First().Second);

        List<bool> res = new();
        foreach (var key in items.Select(x=>x.Key))
        {
            bool val;
            if (keyValMap.TryGetValue(key, out val))
            {
                res.Add(val);
            }
            else
            {
                res.Add(false);
            }
        }
        return res;
    }
    public bool ContainsKey(TKey key) => mapToAllocatorKey.ContainsKey(key);

    public async IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var keys = mapToAllocatorKey.Keys.ToArray();
        var values = dataAllocator.FetchAsAsyncSequence(keys.Select(x => mapToAllocatorKey[x]));

        int i = 0;
        await foreach ( var value in values.WithCancellation(cancellationToken))
        {
            yield return KeyValuePair.Create(keys[i++], value);
        }
    }


}
