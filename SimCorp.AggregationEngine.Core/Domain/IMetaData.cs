using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

namespace SimCorp.AggregationEngine.Core.Domain;

public interface IMetaData
{
    [AggregationLevelAttribute(AggregationLevel.Position)]
    public int? PositionIK { get; set; }

    [AggregationLevelAttribute(AggregationLevel.Security)]
    public int? SecurityIK { get; set; }

    [AggregationLevelAttribute(AggregationLevel.Holding)]
    public int? HoldingIK { get; set; }

    [AggregationLevelAttribute(AggregationLevel.Currency)]
    public string? Currency { get; set; }

    [AggregationLevelAttribute(AggregationLevel.Portfolio)]
    public string? Portfolio { get; set; }

    public string? FreeCode1 { get; set; }
    public string? FreeCode2 { get; set; }   
}
