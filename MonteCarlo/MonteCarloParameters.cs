using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

namespace AggregationEngine.MonteCarlo;

public class MonteCarloParameters : IParameters
{
    public bool Equals(MonteCarloParameters? other)
    {
        if (other == null) return false;
        return other.ConfidenceLevel == ConfidenceLevel;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MonteCarloParameters);
    }

    public override string ToString()
    {
        return ConfidenceLevel.ToString();
    }

    public bool Equals(IParameters? other)
    {
        return Equals(other as MonteCarloParameters);
    }

    [KeyProperty]
    public double ConfidenceLevel {get;set;}
}
