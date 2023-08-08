namespace SimCorp.AggregationEngine.Core.DataCollections;

public interface IMap<TKey, TValue> : IDictionary<TKey, TValue>
{
    void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> values);
    void Remove(IEnumerable<TKey> keys);
}
