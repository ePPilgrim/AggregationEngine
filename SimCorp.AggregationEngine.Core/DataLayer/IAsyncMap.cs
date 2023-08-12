namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAsyncMap<TKey, TValue> : IAsyncEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    IAsyncEnumerable<TValue> Values { get; }
    ICollection<TKey> Keys { get; }
    int Count { get; } 
    Task<IDictionary<TKey, TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token);
    Task<TValue> GetAsync(TKey key, CancellationToken token);
    Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token);
    Task<IList<bool>> RemoveAsync(IList<TKey> keys, CancellationToken token);
    Task<bool> ContainsAsync(KeyValuePair<TKey, TValue> item, CancellationToken token);
    Task<IList<bool>> ContainsAsync(IEnumerable<KeyValuePair<TKey, TValue>> items, CancellationToken token);
    bool ContainsKey(TKey key);
}
