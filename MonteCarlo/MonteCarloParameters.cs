using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

namespace AggregationEngine.MonteCarlo;

public class MonteCarloParameters : IParameters
{
    public bool Equals(IParameters? other)
    {
        var otherMCParameters = other as MonteCarloParameters;
        if (ReferenceEquals(null, otherMCParameters)) return false;
        return otherMCParameters.ConfidenceLevel == this.ConfidenceLevel;
    }

    [KeyProperty]
    public double ConfidenceLevel {get;set;}
}
