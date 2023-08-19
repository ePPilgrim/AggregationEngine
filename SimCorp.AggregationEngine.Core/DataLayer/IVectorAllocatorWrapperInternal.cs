using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IVectorAllocatorWrapperInternal<TVector> : IMetaData where TVector : IAggregationPosition
{
    public Task<TVector> GetVectorAsync(CancellationToken token);
    public TVector GetVector();
    public DateTime TimeStamp { get; }
    public IMetaData MetaData { get; }
    public IKey Key { get; }
}
