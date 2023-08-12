using SimCorp.AggregationEngine.Core.DataLayer;
using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core;

internal struct VectorWrapper<T> : IAggregationPosition where T : struct, IAggregationPosition
{
    private readonly T vector;
    private readonly IAsyncDataAllocator<T> allocator;

    public VectorWrapper(T vector, IAsyncDataAllocator<T> vectorCache)
    {
        this.vector = vector;
        allocator = vectorCache;
    }
    public void CombineWith(IEnumerable<IAggregationPosition> aggregationPositions)
    {
        vector.CombineWith(aggregationPositions);
    }

    public bool Equals(IAggregationPosition? other)
    {
        if(other == null) return false;
        return vector.Equals(other);
    }

    public async Task<T> GetAsyncVector(CancellationToken token)
    {
        return await allocator.GetAsync(AllocatorKey, token);
    }
    public DateTime TimeStamp { get; set; }
    public string AllocatorKey { get; set; }
}
