using SimCorp.AggregationEngine.Core.Key;
using System.Collections.Concurrent;

namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class DefaultAsyncMapInternal<TKey, TValue> : IAsyncMapInternal<TKey, TValue> where TKey : notnull, IKey
{
    private readonly ConcurrentDictionary<TKey, string> keyMap;
    private readonly IAsyncExternalDataAllocator<TValue> dataAllocator;
    private readonly IDataLayerFactory<TKey> factory;

    public DefaultAsyncMapInternal(IAsyncExternalDataAllocator<TValue> dataAllocator, IDataLayerFactory<TKey> factory)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
        keyMap = new ConcurrentDictionary<TKey, string>();
        this.factory = factory;
    }

    public ICollection<TKey> Keys => keyMap.Keys;
    public int Count => Keys.Count;

    public IEnumerable<TValue> GetAllValues()
    {
        var res = dataAllocator.FetchAtOnceAsync(keyMap.Values.ToHashSet(), default(CancellationToken));
        return res.Result.Values;
    } 

    public async Task<IDictionary<TKey,TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token)
    {
        if(!keys.All(x => keyMap.ContainsKey(x)))
        {
            throw new KeyNotFoundException($"Not all required keys are found.");
        }
        var allocKeys = keys.Select(x => keyMap[x]).ToHashSet();
        var fetchedValues = await dataAllocator.FetchAtOnceAsync(allocKeys, token);
        return keys.ToDictionary(x => x, x => fetchedValues[keyMap[x]]);
    }

    public async Task<TValue> GetAsync(TKey key, CancellationToken token)
    {
        if (!keyMap.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Required key {key} is not found.");
        }

        return await dataAllocator.GetAsync(keyMap[key], token);
    }

    public async Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token)
    {
        var allocEnumerator = positions.Select(x => KeyValuePair.Create(x.Key.ToUniqueString(), x.Value));
        await dataAllocator.SetAsync(allocEnumerator, token);
        var keyToStringKeyMapping = positions.Where(x => !keyMap.ContainsKey(x.Key)).Select(x => KeyValuePair.Create(x.Key, x.Key.ToUniqueString()));
        foreach(var item in keyToStringKeyMapping.Distinct())
        {
            keyMap.TryAdd(item.Key, item.Value);
        }
    }

    public async Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TKey> keys, CancellationToken token)
    {
        var keyToStringKeyItems = keys.Where(x => keyMap.ContainsKey(x)).Select(x => new { Key = x, AllocKey = keyMap[x] }).ToList();
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
        if(!keyMap.ContainsKey(item.Key))
        {
            return false;
        }
        var res = await dataAllocator.ContainsAsync(new[] { KeyValuePair.Create(keyMap[item.Key], item.Value)}, token);
        return res.First();
    }

    public async Task<IList<bool>> ContainsAsync(IEnumerable<KeyValuePair<TKey, TValue>> items, CancellationToken token)
    {
        var expanded = items.Where(x => keyMap.ContainsKey(x.Key)).Select(x => new { x.Key, Value = KeyValuePair.Create(keyMap[x.Key], x.Value) });
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
    public bool ContainsKey(TKey key) => keyMap.ContainsKey(key);

    public async IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var keys = keyMap.Keys.ToArray();
        var values = dataAllocator.FetchAsAsyncSequence(keys.Select(x => keyMap[x]));

        int i = 0;
        await foreach ( var value in values.WithCancellation(cancellationToken))
        {
            yield return KeyValuePair.Create(keys[i++], value);
        }
    }

    public IAsyncEnumerable<TValue> GetAllValuesAsync()
    {
        return dataAllocator.FetchAsAsyncSequence(keyMap.Select(x => x.Value));
    }

    public async Task TouchAsync(TKey key, CancellationToken token)
    {
        if (!keyMap.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Required key {key} is not found.");
        }
        await dataAllocator.TouchAsync(new[] { keyMap[key] }, token);
    }

    public async Task TouchAsync(HashSet<TKey> keys, CancellationToken token)
    {
        if (keys.Any(x => !keyMap.ContainsKey(x)))
        {
            throw new KeyNotFoundException($"Some of the required keys are not found.");
        }
        await dataAllocator.TouchAsync(keys.Select(x => keyMap[x]), token);
    }

    public void Dispose()
    {
        var res = dataAllocator.RemoveAsync(keyMap.Values, CancellationToken.None);
        res.Wait();
        keyMap.Clear();
    }

    public async Task<bool> TryShallowInsert(TKey key, CancellationToken token)
    {
        if (keyMap.ContainsKey(key)) return true;
        var res = await dataAllocator.ContainsKeyAsync(key.ToUniqueString(), token);
        if (res == false) return false;
        keyMap.TryAdd(key, key.ToUniqueString());
        await dataAllocator.TouchAsync(new[] {key.ToUniqueString() }, token);
        return true;
    }

    public object Clone()
    {
        var domainLayer = factory.Create<TKey>();
        foreach(var key in Keys)
        {
            domainLayer.TryShallowInsert(key, CancellationToken.None).Wait();   
        }
        return domainLayer;
    }
}
