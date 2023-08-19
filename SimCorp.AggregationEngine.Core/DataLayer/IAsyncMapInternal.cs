﻿using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IAsyncMapInternal<TKey, TValue> : IEnumerable<KeyValuePair<TKey, Task<TValue>>>, IDisposable, ICloneable where TKey : IKey
{
    ICollection<TKey> Keys { get; }
    IEnumerable<TValue> GetAllValues();
    Task<TValue> GetAsync(TKey key, CancellationToken token);
    Task<IDictionary<TKey, TValue>> GetAsync(HashSet<TKey> keys, CancellationToken token);
    Task AddAsync(TKey key, TValue value, CancellationToken token);
    Task<bool> TryShallowAddAsync(TKey key, CancellationToken token);
    Task UpdateOrAddAsync(IEnumerable<KeyValuePair<TKey, TValue>> positions, CancellationToken token);
    Task RemoveAsync(IEnumerable<TKey> keys, CancellationToken token);
}