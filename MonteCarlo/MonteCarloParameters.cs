using SimCorp.AggregationEngine.Core.Domain;

namespace AggregationEngine.MonteCarlo;

public class MonteCarloParameters : IParameters
{
    public bool Equals(IParameters? other)
    {
        var otherMCParameters = other as MonteCarloParameters;
        if (ReferenceEquals(null, otherMCParameters)) return false;
        return otherMCParameters.ConfidenceLevel == this.ConfidenceLevel;
    }

    public double ConfidenceLevel {get;set;}
}
