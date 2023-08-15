using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal class PositionAllocatorWrapperInternal<T> : IAggregationPosition where T : struct, IAggregationPosition
{
    private readonly IMetaData metaData;
    private readonly IAsyncDataAllocator<T> allocator;
    private readonly 

    public PositionAllocatorWrapperInternal(T vector, IAsyncDataAllocator<T> vectorCache)
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
        if (other == null) return false;
        return vector.Equals(other);
    }

    public async Task<T> GetVectorAsync(CancellationToken token)
    {
        return await allocator.GetAsync(AllocatorKey, token);
    }
    public DateTime TimeStamp { get; set; }
    public string AllocatorKey { get; set; }

    public IMetaData MetaData => throw new NotImplementedException();

    public IVector Values => throw new NotImplementedException();
}
