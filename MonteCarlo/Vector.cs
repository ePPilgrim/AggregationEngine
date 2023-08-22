using SimCorp.AggregationEngine.Core.Domain;

namespace AggregationEngine.MonteCarlo;

public class Vector : IVector
{
    public Vector()
    {
        
    }
    public Vector(Vector sv)
    {
        if(sv.Scenarious != null)
        Scenarious = sv.Scenarious.ToArray();
    }
    public double[]? Scenarious { get; set; }
}
