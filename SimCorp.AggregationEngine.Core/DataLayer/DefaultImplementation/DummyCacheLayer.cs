using System.Text.Json;

namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class DummyCacheLayer<T> : ICacheLayer<T>
{
    private readonly Dictionary<string, CacheNode> data;

    public DummyCacheLayer()
    {
        data = new Dictionary<string, CacheNode>();
    }
    public T Get(string key)
    {
        if (data.ContainsKey(key))
        {
            return JsonSerializer.Deserialize<T>(data[key].Data);
        }
        return default(T);
    }
    public void Set(string key, T value)
    {
        int refCount = (data.ContainsKey(key)) ? data[key].RefCount : 1;
        data[key] = new CacheNode()
        {
            Data = JsonSerializer.SerializeToUtf8Bytes<T>(value),
            TimeStamp = DateTime.UtcNow,
            RefCount = refCount
        };
    }
    public void Clear(string key)
    {
        if(data.ContainsKey(key))
        {
            if (data[key].RefCount == 1)
            {
                data.Remove(key);
                return;
            }
            data[key].RefCount--;
        }
    }

    public DateTime? GetTimeStamp(string key)
    {
        if (data.ContainsKey(key))
        {
            return data[key].TimeStamp;
        }
        return null;
    }

    public bool Contains(string key, T value)
    {
        throw new NotImplementedException();
    }
}
