using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer;

internal interface IVectorAllocatorWrapperInternal<TVector> : IMetaData where TVector : IAggregationPosition
{
    public Task<TVector> GetVectorAsync(CancellationToken token);
    public TVector GetVector();
    public DateTime TimeStamp { get; }
    public IMetaData MetaData { get; }
    public IKey Key { get; }
    string GetExternalAllocatorID();
}
