using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal class AllocatorWrapperInternal<TVector> : IMetaData where TVector : IAggregationPosition
{
    private readonly IMetaData metaData;
    private readonly IAsyncMapInternal<IKey, TVector> allocator;

    public AllocatorWrapperInternal(IMetaData metaData, IKey key, IAsyncMapInternal<IKey, TVector> allocator)
    {
        this.metaData = metaData;
        this.allocator = allocator;
        this.Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    public async Task<TVector> GetVectorAsync(CancellationToken token)
    {
        return await allocator.GetAsync(Key, token);
    }
    public DateTime TimeStamp { get; set; }

    public IMetaData MetaData => metaData;
    public IKey Key { get; }

    public IVector Values
    {
        get
        {
            var res = allocator.GetAsync(Key, CancellationToken.None);
            return res.Result.Values;
        }
    }
}
