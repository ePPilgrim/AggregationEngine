using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

namespace AggregationEngine.MonteCarlo;

public interface IDummyMetaData : IMetaData
{
    [AggregationLevel(AggregationLevel.Position)]
    public int? PositionIK { get; set; }

    [AggregationLevel(AggregationLevel.Security)]
    public int? SecurityIK { get; set; }

    [AggregationLevel(AggregationLevel.Portfolio)]
    public string? Portfolio { get; set; }
    public string? FreeCode1 { get; set; }
    public string? FreeCode2 { get; set; }

    [AggregationLevel(AggregationLevel.Holding)]
    public int? HoldingIK { get; set; }

    [AggregationLevel(AggregationLevel.Currency)]
    public string? Currency { get; set; }

    [AggregationLevel(AggregationLevel.InstrumentType)]
    public int? InstrumentType { get; set; }

    public string? FreeCode3 { get; set; }
    public int FreeCode4 { get; set; }
}
