namespace SimCorp.AggregationEngine.Core.DataLayer;

public interface IAsyncExternalDataAllocator<T>
{
    Task<T> GetAsync(string key, CancellationToken token);
    IAsyncEnumerable<T> FetchAsAsyncSequence(IEnumerable<string> keys);
    Task<IDictionary<string,T>> FetchAtOnceAsync(HashSet<string> keys, CancellationToken token);
    Task SetAsync(IEnumerable<KeyValuePair<string, T>> values, CancellationToken token);
    Task<IEnumerable<bool>> RemoveAsync(IEnumerable<string> keys, CancellationToken token);
    Task<IEnumerable<bool>> ContainsAsync(IEnumerable<KeyValuePair<string, T>> values, CancellationToken token);
    Task<bool> ContainsKeyAsync(string key, CancellationToken token);   
    Task<IEnumerable<DateTime>> GetTimeStampsAsync(IEnumerable<string> keys, CancellationToken token);
    Task TouchAsync(IEnumerable<string> keys, CancellationToken token);
    Task FlushAsync(CancellationToken token);
}
