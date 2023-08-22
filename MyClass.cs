using SimCorp.AggregationEngine.Core.Domain;

namespace AggregationEngine;

internal class MyClass : IMyClass
{
    
    public int? PositionIK { get; set; }
    public int? SecurityIK { get; set; }
    public int? HoldingIK { get; set; }
    public string? Currency { get; set; }
    public string? Portfolio { get=>p + SomeValue; set=>p = value; }
    public string? FreeCode1 { get; set; }
    public string? FreeCode2 { get; set; }
    public string? SomeValue { get; set; }
    private string? p = "";
}

internal class MyMyClass : IMyMyClass
{

    public int? PositionIK { get; set; }
    public int? SecurityIK { get; set; }
    public int? HoldingIK { get; set; }
    public string? Currency { get; set; }
    public string? Portfolio { get => pp +"-HHH-" + SomeValue; set => pp = value; }
    public string? FreeCode1 { get; set; }
    public string? FreeCode2 { get; set; }
    public string? SomeValue { get; set; }
    private string? pp = "";

}

public interface IMyMyClass : IMetaData
{
    public string SomeValue { get; set; }
}

public interface IMyClass : IMetaData
{
    public string SomeValue { get; set; }   
}
