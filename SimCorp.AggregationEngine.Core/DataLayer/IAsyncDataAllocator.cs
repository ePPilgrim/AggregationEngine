namespace SimCorp.AggregationEngine.Core.DataLayer;

public interface IAsyncDataAllocator<T>
{
    IAsyncEnumerable<T> Get(IEnumerable<string> keys);
    Task Set(IEnumerable<KeyValuePair<string, T>> values);
    Task<IEnumerable<bool>> Remove(IEnumerable<string> keys);
    Task<IEnumerable<bool>> Contains(IEnumerable<KeyValuePair<string, T>> values);
    Task<IEnumerable<DateTime>> GetTimeStamps(IEnumerable<string> keys);
    Task Flush();
}
