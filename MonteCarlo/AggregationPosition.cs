using SimCorp.AggregationEngine.Core.Domain;

namespace AggregationEngine.MonteCarlo;

public class AggregationPosition : IAggregationPosition
{
    public IMetaData MetaData => throw new NotImplementedException();

    public IScalingData ScalingData => throw new NotImplementedException();

    public IVector Values => throw new NotImplementedException();
}
