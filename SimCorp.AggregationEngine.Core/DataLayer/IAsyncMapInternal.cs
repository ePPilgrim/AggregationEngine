using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAsyncMapInternal<TKey, TValue> : IEnumerable<KeyValuePair<TKey, Task<TValue>>>, IDisposable, ICloneable where TKey : IKey
{
    Task AddAsync(TKey key, TValue value, CancellationToken token);
    Task<bool> TryShallowInsert(TKey key, CancellationToken token);
    Task<IEnumerable<bool>> RemoveAsync(IEnumerable<TKey> keys, CancellationToken token);
    IEnumerable<TValue> GetAllValues();
    Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token);
    Task<TValue> GetAsync(TKey key, CancellationToken token);





    ICollection<TKey> Keys { get; }
    int Count { get; }
    
    IAsyncEnumerable<TValue> GetAllValuesAsync();
    
    Task<IDictionary<TKey, TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token);
    
   
    
    Task<bool> ContainsAsync(KeyValuePair<TKey, TValue> item, CancellationToken token);
    Task<IList<bool>> ContainsAsync(IEnumerable<KeyValuePair<TKey, TValue>> items, CancellationToken token);
    Task TouchAsync(TKey key, CancellationToken token);
    Task TouchAsync(HashSet<TKey> keys, CancellationToken token);   
    bool ContainsKey(TKey key);
}
