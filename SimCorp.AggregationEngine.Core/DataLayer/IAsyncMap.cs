namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAsyncMap<TKey, TValue> : IAsyncEnumerable<KeyValuePair<TKey, TValue>>, IDisposable where TKey : notnull
{
    IEnumerable<TValue> Values { get; }
    ICollection<TKey> Keys { get; }
    int Count { get; }
    IAsyncEnumerable<TValue> GetAllValuesAsync();
    Task<IDictionary<TKey, TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token);
    Task<TValue> GetAsync(TKey key, CancellationToken token);
    Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token);
    Task<IList<bool>> RemoveAsync(IList<TKey> keys, CancellationToken token);
    Task<bool> ContainsAsync(KeyValuePair<TKey, TValue> item, CancellationToken token);
    Task<IList<bool>> ContainsAsync(IEnumerable<KeyValuePair<TKey, TValue>> items, CancellationToken token);
    bool ContainsKey(TKey key);
}
