namespace SimCorp.AggregationEngine.Core.DataLayer.DefaultImplementation;

internal record AllocNode
{
    public AllocNode()
    {
        Data = Array.Empty<byte>();
    }
    public byte[] Data { get; set; }
    public DateTime TimeStamp { get; set; }
    public int RefCount { get; set; }
}
