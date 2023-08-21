namespace SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;

internal class DefaultOptimizationPolicyInternal : IOptimizationPolicyInternal
{
    private int vectorChankSize = 50;
    public int VectorChankSize { get => vectorChankSize; set => vectorChankSize = value; }
}
