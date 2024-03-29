﻿using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

namespace AggregationEngine.MonteCarlo;

public class MetaData : IDummyMetaData
{
    public MetaData()
    {
        
    }
    public MetaData(MetaData otherMetadata)
    {
        PositionIK = otherMetadata.PositionIK;
        SecurityIK = otherMetadata.SecurityIK;
        HoldingIK = otherMetadata.HoldingIK;
        Currency = otherMetadata.Currency;
        Portfolio  = otherMetadata.Portfolio;
        InstrumentType = otherMetadata.InstrumentType;
        FreeCode1 = otherMetadata.FreeCode1;
        FreeCode2 = otherMetadata.FreeCode2;
        FreeCode3 = otherMetadata.FreeCode3;
        FreeCode4 = otherMetadata.FreeCode4;    
    }
    public int? PositionIK { get; set; }
    public int? SecurityIK { get; set; }
    public int? HoldingIK { get; set; }
    public string? Currency { get; set; }
    public string? Portfolio { get; set; }
    public string? FreeCode1 { get; set; }
    public string? FreeCode2 { get; set; }
    public string? FreeCode3 { get; set; }
    public int? InstrumentType { get ; set; }
    public int FreeCode4 { get; set; }
}
