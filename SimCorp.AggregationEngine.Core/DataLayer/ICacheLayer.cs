namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface ICacheLayer<T>
{
    T Get(string key);
    void Set(string key, T value);
    void Clear(string key);
    bool Contains(string key, T value);
    DateTime? GetTimeStamp(string key);
}
