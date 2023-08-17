using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IKeyPropertySelector
{
    KeyValuePair<string, string?> GetPropertyWithAggregationLevel<T>(T value, AggregationLevel aggregationLevel);
    IReadOnlyDictionary<string, string?> GetPropertiesWithKeyAttribute<T>(T value);
}
