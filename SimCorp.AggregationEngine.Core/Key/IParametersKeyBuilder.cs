using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IParametersKeyBuilder<TKey> where TKey : IKey
{
    TKey Build(IParameters parameters);
}
