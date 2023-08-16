using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IKeyInterfacePropertySelector<T> 
{
    KeyValuePair<string, string?> GetPropertyWithAggregationLevel(T value, AggregationLevel aggregationLevel);
    IReadOnlyDictionary<string, string?> GetPropertiesWithKeyAttribute(T value);
}
