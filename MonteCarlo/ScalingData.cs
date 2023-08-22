using SimCorp.AggregationEngine.Core.Domain;

namespace AggregationEngine.MonteCarlo;

public class ScalingData : IScalingData
{
    public ScalingData()
    {
        
    }
    public ScalingData(ScalingData sc)
    {
        Nominal = sc.Nominal;
    }
    public double Nominal { get; set; }
}
