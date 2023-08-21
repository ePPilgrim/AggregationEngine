namespace AggregationEngine;

public class MetaData : IMetaData3, ISomeOtherData
{
    private readonly string s1 = "HHHHH+";
    private readonly string s2 = "VVVVVV";
    private string s = "";
    public string? PortfolioName { get; set; }
    public int SecurityIK { get; set; }
    public double Currency { get; set; }
    public int HoldingIK { get; set; }

    [AggregationLevel(AggregationLevel.Currency)]
    public string? Name { get => s1 + s2; set => s = value; }
    public string Name2 { get; set; }
    public string Name3 { get; set; }

    protected string Name4 { get; set; }
    public string Name5 { set => s = value; }
}

public interface ISomeOtherData
{
    string? Name { get; set; }
    string Name2 { get; set; }
}

public interface IMetaData3 : IMetaData2
{
    string Name3 { get; set; }
    string Name2 { get; set; }
}

public interface IMetaData2 : IMetaData
{
   string Name2 { get; set; }
}

public interface IMetaData
{
    [AggregationLevel(AggregationLevel.Portfolio)]
    public string? PortfolioName { get; set; }
    [AggregationLevel(AggregationLevel.Security)]
    public int SecurityIK { get; set; }
    [AggregationLevel(AggregationLevel.Currency)]
    public int Currency { get; set; }
    public int HoldingIK { get; set; }  
    public string? Name { get; set; }    
}




