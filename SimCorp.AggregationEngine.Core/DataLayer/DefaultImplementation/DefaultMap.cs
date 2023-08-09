using SimCorp.AggregationEngine.Core.DataCollections;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal abstract class DefaultMap<TKey, TValue> : IMap<TKey, TValue> where TKey : class, IEquatable<TKey>
{
    private readonly IDictionary<TKey, string> mapToCacheKey;
    private readonly ICacheLayer<TValue> cache;
    private readonly IKeyToStringConvertor<TKey> keyToString; //key helper

    public DefaultMap(ICacheLayer<TValue> cache, IKeyToStringConvertor<TKey> keyToString)
    {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.keyToString = keyToString ?? throw new ArgumentNullException( nameof(keyToString));
    }

    public TValue this[TKey key] { 
        
        get 
        {
            if (!mapToCacheKey.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Value with key {key} is not found");
            }
            return cache.Get(mapToCacheKey[key]);
        }

        set
        {
            if(!mapToCacheKey.ContainsKey(key))
            {
                mapToCacheKey.Add(key, keyToString.KeyToString(key));             
            }
            cache.Set(mapToCacheKey[key], value);
        }
    }

    public ICollection<TKey> Keys => mapToCacheKey.Keys;

    public ICollection<TValue> Values => mapToCacheKey.Select(x => cache.Get(x.Value)).ToList();

    public int Count => mapToCacheKey.Count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        mapToCacheKey.Add(key, keyToString.KeyToString(key));
        cache.Set(mapToCacheKey[key], value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Remove(mapToCacheKey.Keys);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if(!mapToCacheKey.ContainsKey(item.Key))
        {
            return false;
        }
        if(!cache.Contains(mapToCacheKey[item.Key], item.Value))
        {
            return false;
        }
        return true;
    }

    public bool ContainsKey(TKey key)
    {
        return mapToCacheKey.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach(var val in mapToCacheKey)
        {
            array[arrayIndex++] = KeyValuePair.Create(val.Key, cache.Get(val.Value));
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var val in mapToCacheKey)
        {
           yield return KeyValuePair.Create(val.Key, cache.Get(val.Value));
        }
    }

    public void Remove(IEnumerable<TKey> keys)
    {
        foreach(var key in keys)
        {
            Remove(key);
        }
    }

    public bool Remove(TKey key)
    {
        if (mapToCacheKey.ContainsKey(key))
        {
            cache.Clear(mapToCacheKey[key]);
            mapToCacheKey.Remove(key);
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        string? cacheKey;
        if (!mapToCacheKey.TryGetValue(item.Key, out cacheKey))
        {
            return false;
        }
        if (!cache.Contains(cacheKey, item.Value))
        {
            return false;
        }
        cache.Clear(cacheKey);
        mapToCacheKey.Remove(item.Key);
        return true;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        value = default(TValue);
        string? cacheKey;
        if(mapToCacheKey.TryGetValue(key, out cacheKey))
        {
            value = cache.Get(cacheKey);
        }
        return false;
    }

    public void UpdateOrAdd(IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var val in this)
        {
            yield return (object)val;
        }
    }
}
