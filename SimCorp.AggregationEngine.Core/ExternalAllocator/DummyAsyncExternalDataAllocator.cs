using System.Collections.Concurrent;
using System.Text.Json;

namespace SimCorp.AggregationEngine.Core.ExternalAllocator;

internal class DummyAsyncExternalDataAllocator<T> : IAsyncExternalDataAllocator<T>
{
    private readonly ConcurrentDictionary<string, AllocNode> data;
    private readonly int Id;
    private readonly Random random;
    private int randomReadFrom = 10; //millisecond
    private int randomReadTo = 90; //millisecond

    private int randomWriteFrom = 10; //millisecond
    private int randomWriteTo = 110; //millisecond

    public DummyAsyncExternalDataAllocator(int Id = 0)
    {
        random = new Random();
        data = new ConcurrentDictionary<string, AllocNode>();
        this.Id = Id;
    }

    public async Task<T> GetAsync(string key, CancellationToken token)
    {
        int interval = random.Next(randomReadFrom, randomReadTo);
        await Task.Delay(interval, token);
        AllocNode? node;
        if (data.TryGetValue(key, out node))
        {
            return await Task.FromResult(JsonSerializer.Deserialize<T>(node.Data)!);
        }
        return default(T);
    }

    public async IAsyncEnumerable<T> FetchAsAsyncSequence(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            int interval = random.Next(randomReadFrom, randomReadTo);
            await Task.Delay(interval);
            AllocNode? node;
            if (data.TryGetValue(key, out node))
            {
                yield return JsonSerializer.Deserialize<T>(node.Data)!;
            }
            else
            {
                yield return default;
            }
        }
    }

    public async Task<IDictionary<string, T>> FetchAtOnceAsync(HashSet<string> keys, CancellationToken token)
    {
        Dictionary<string, T> dict = new();
        foreach (var key in keys)
        {
            int interval = random.Next(randomReadFrom, randomReadTo);
            await Task.Delay(interval, token);
            AllocNode? node;
            if (data.TryGetValue(key, out node))
            {
                dict[key] = JsonSerializer.Deserialize<T>(node.Data)!;
            }
            else
            {
                dict[key] = default;
            }
        }
        return dict;
    }

    public async Task SetAsync(IEnumerable<KeyValuePair<string, T>> values, CancellationToken token)
    {
        foreach (var item in values)
        {
            int interval = random.Next(randomWriteFrom, randomWriteTo);
            await Task.Delay(interval, token);
            AllocNode? node;
            if (data.TryGetValue(item.Key, out node))
            {
                node.Data = JsonSerializer.SerializeToUtf8Bytes(item.Value);
                node.TimeStamp = DateTime.UtcNow;
                node.RefCount++;
            }
            else
            {
                node = new AllocNode()
                {
                    Data = JsonSerializer.SerializeToUtf8Bytes(item.Value),
                    TimeStamp = DateTime.UtcNow,
                    RefCount = 1
                };
            }
            data.AddOrUpdate(item.Key, node, (x, y) => y);
        }
    }

    public async Task<IEnumerable<bool>> RemoveAsync(IEnumerable<string> keys, CancellationToken token)
    {
        var res = keys.Select(key => data.ContainsKey(key));
        foreach (var key in keys.Where(x => data.ContainsKey(x)))
        {
            int interval = random.Next(randomReadFrom, randomReadTo);
            await Task.Delay(interval, token);
            AllocNode? node;
            if (!data.TryGetValue(key, out node))
            {
                continue;
            }
            node.RefCount--;
            if (node.RefCount == 0)
            {
                data.TryRemove(KeyValuePair.Create(key, node));
            }
        }
        return await Task.FromResult(res);
    }

    public async Task<bool> ContainsKeyAsync(string key, CancellationToken token)
    {
        int interval = random.Next(randomReadFrom, randomReadTo);
        await Task.Delay(interval, token);
        AllocNode? node;
        if (data.TryGetValue(key, out node))
        {
            return true;
        }
        return false;
    }

    public async Task<IEnumerable<DateTime>> GetTimeStampsAsync(IEnumerable<string> keys, CancellationToken token)
    {
        List<DateTime> stamps = new List<DateTime>();
        foreach (var key in keys)
        {
            int interval = random.Next(randomReadFrom, randomReadTo);
            await Task.Delay(interval, token);
            AllocNode? node;
            if (data.TryGetValue(key, out node))
            {
                stamps.Add(node.TimeStamp);
            }
            else
            {
                stamps.Add(default);
            }
        }
        return stamps;
    }

    public async Task TouchAsync(IEnumerable<string> keys, CancellationToken token)
    {
        foreach (var key in keys)
        {
            int interval = random.Next(randomReadFrom, randomReadTo);
            await Task.Delay(interval, token);
            AllocNode? node;
            if (data.TryGetValue(key, out node))
            {
                node.RefCount++;
            }
        }
    }

    public async Task FlushAsync(CancellationToken token)
    {
        int interval = random.Next(randomReadFrom, randomReadTo);
        await Task.Delay(interval, token);
        data.Clear();

    }

    public string GetId()
    {
        return Id.ToString();
    }

}