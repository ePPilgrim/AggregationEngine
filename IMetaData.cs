namespace AggregationEngine;

public struct MetaData : IMetaData
{
    public string? PortfolioName { get; set; }
    public int SecurityIK { get; set; }
    public double Currency { get; set; }
    public int HoldingIK { get; set; }
    public string? Name { get; set; }
}

public interface IMetaData3 : IMetaData2
{
    string MyNamed { get; set; }
}

public interface IMetaData2 : IMetaData
{
   string MyName { get; set; }
}

public interface IMetaData
{
    [AggregationLevel(AggregationLevel.Portfolio)]
    public string? PortfolioName { get; set; }
    [AggregationLevel(AggregationLevel.Security)]
    public int SecurityIK { get; set; }
    [AggregationLevel(AggregationLevel.Currency)]
    public double Currency { get; set; }
    public int HoldingIK { get; set; }  
    public string? Name { get; set; }    
}


