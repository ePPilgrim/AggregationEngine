namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal class CacheNode
{
    public byte[]? Data { get; set; }
    public DateTime TimeStamp { get; set; }
    public int RefCount { get; set; }
}
