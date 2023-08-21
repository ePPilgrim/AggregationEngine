using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Key;
using System.Collections;
using System.Collections.Concurrent;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;

internal class DefaultAsyncMapInternal<TKey, TValue> : IAsyncMapInternal<TKey, TValue> where TKey : notnull, IKey
{
    private readonly ConcurrentDictionary<TKey, string> keyMap;
    private readonly IAsyncExternalDataAllocator<TValue> dataAllocator;
    private readonly IDataLayerFactory<TValue> factory;

    public DefaultAsyncMapInternal(IAsyncExternalDataAllocator<TValue> dataAllocator, IDataLayerFactory<TValue> factory)
    {
        this.dataAllocator = dataAllocator ?? throw new ArgumentNullException(nameof(dataAllocator));
        keyMap = new ConcurrentDictionary<TKey, string>();
        this.factory = factory;
    }

    public ICollection<TKey> Keys => keyMap.Keys;

    public IEnumerable<TValue> GetAllValues()
    {
        var res = dataAllocator.FetchAtOnceAsync(keyMap.Values.ToHashSet(), CancellationToken.None);
        return res.Result.Values;
    }

    public async Task<TValue> GetAsync(TKey key, CancellationToken token)
    {
        if (!keyMap.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Required key {key} is not found.");
        }

        return await dataAllocator.GetAsync(keyMap[key], token);
    }

    public async Task<IDictionary<TKey, TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token)
    {
        if (!keys.All(x => keyMap.ContainsKey(x)))
        {
            throw new KeyNotFoundException($"Not all required keys are found.");
        }
        var allocKeys = keys.Select(x => keyMap[x]).ToHashSet();
        var fetchedValues = await dataAllocator.FetchAtOnceAsync(allocKeys, token);
        return keys.ToDictionary(x => x, x => fetchedValues[keyMap[x]]);
    }

    public async Task AddAsync(TKey key, TValue value, CancellationToken token)
    {
        string externalAllocatorKey = key.ToUniqueString();
        await dataAllocator.SetAsync(new[] { KeyValuePair.Create(externalAllocatorKey, value) }, token);
        keyMap[key] = externalAllocatorKey;
    }

    public async Task<bool> TryShallowAddAsync(TKey key, CancellationToken token)
    {
        if (keyMap.ContainsKey(key)) return true;
        string externalAllocatorKey = key.ToUniqueString();
        var res = await dataAllocator.ContainsKeyAsync(externalAllocatorKey, token);
        if (res == false) return false;
        await dataAllocator.TouchAsync(new[] { externalAllocatorKey }, token);
        keyMap[key] = externalAllocatorKey;
        return true;
    }

    public async Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token)
    {
        var allocEnumerator = positions.Select(x => KeyValuePair.Create(x.Key.ToUniqueString(), x.Value));
        await dataAllocator.SetAsync(allocEnumerator, token);
        var keyToStringKeyMapping = positions.Where(x => !keyMap.ContainsKey(x.Key)).Select(x => KeyValuePair.Create(x.Key, x.Key.ToUniqueString()));
        foreach (var item in keyToStringKeyMapping.Distinct())
        {
            keyMap[item.Key] = item.Value;
        }
    }

    public async Task RemoveAsync(IEnumerable<TKey> keys, CancellationToken token)
    {
        var validKeySet = keys.Where(x => keyMap.ContainsKey(x)).ToHashSet();
        var externalAllocatorKeys = validKeySet.Select(x => keyMap[x]).ToList();
        foreach (var key in validKeySet)
        {
            keyMap.Remove(key, out _);
        }

        await dataAllocator.RemoveAsync(validKeySet.Select(x => keyMap[x]), token);
    }

    public void Dispose()
    {
        var res = dataAllocator.RemoveAsync(keyMap.Values, CancellationToken.None);
        res.Wait();
        keyMap.Clear();
    }

    public object Clone()
    {
        var domainLayer = factory.Create<TKey>(dataAllocator);
        foreach (var key in Keys)
        {
            domainLayer.TryShallowAddAsync(key, CancellationToken.None).Wait();
        }
        return domainLayer;
    }

    public IEnumerator<KeyValuePair<TKey, Task<TValue>>> GetEnumerator()
    {
        foreach (var key in Keys)
        {
            var value = dataAllocator.GetAsync(keyMap[key], CancellationToken.None);
            yield return KeyValuePair.Create(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public string GetExternalAllocatorID()
    {
        return dataAllocator.GetId();
    }
}
