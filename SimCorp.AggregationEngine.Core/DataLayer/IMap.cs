namespace SimCorp.AggregationEngine.Core.DataCollections;

internal interface IMap<TKey, TValue> : IDictionary<TKey, TValue>
{
    void UpdateOrAdd(IEnumerable<KeyValuePair<TKey, TValue>> values);
    void Remove(IEnumerable<TKey> keys);
}
