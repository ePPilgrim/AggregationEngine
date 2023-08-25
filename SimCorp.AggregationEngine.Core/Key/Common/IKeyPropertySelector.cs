using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Common;

public interface IKeyPropertySelector
{
    KeyValuePair<string, string?>? GetPropertyWithAggregationLevel<T>(T value, AggregationLevel aggregationLevel) where T : IMetaData;
    IReadOnlyDictionary<string, string?> GetPropertiesWithKeyAttribute<T>(T value) where T : IParameters;
}
