using System.Collections.Concurrent;
using System.Text.Json;

namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class DummyAsyncExternalDataAllocator<T> : IAsyncExternalDataAllocator<T>
{
    private readonly ConcurrentDictionary<string, AllocNode> data;

    public DummyAsyncExternalDataAllocator()
    {
        data = new Dictionary<string, AllocNode>();
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
        data[key] = new AllocNode()
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

    public Task<IEnumerable<T>> Get(IEnumerable<string> keys)
    {

        if (data.ContainsKey(key))
        {
            return JsonSerializer.Deserialize<T>(data[key].Data);
        }
        return default(T);
    }

    public void Set(IEnumerable<KeyValuePair<string, T>> values)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<bool> Remove(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<bool> Contains(IEnumerable<KeyValuePair<string, T>> values)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateTime> GetTimeStamps(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }
}
