namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAsyncMap<TKey, TValue> : IAsyncEnumerable<KeyValuePair<TKey, TValue>>
{
    Task<IAsyncEnumerable<TValue>> Values { get; }
    IEnumerable<TKey> Keys { get; }
    Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token);
    Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TKey> keys, CancellationToken token);
    Task<bool> Contains(KeyValuePair<TKey, TValue> item);
    bool ContainsKey(TKey key);
}
