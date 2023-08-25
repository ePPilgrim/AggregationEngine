// See https://aka.ms/new-console-template for more information

using AggregationEngine;
using AggregationEngine.MonteCarlo;
using Key.UnitTest.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimCorp.AggregationEngine.Core;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

var aggregationPosition = new DummyAggregatPosition()
{
    DummyCurrency = "EUR",
    DummyFreeCode1 = "AggPosFreeCode1",
    DummyFreeCode2 = "AggPosFreeCode2",
    DummyHoldingIK = 1,
    DummyInstrumentType = DummyInstrumentType.Bond,
    DummyPortfolio = "AggPosPortfolio",
    DummyPositionIK = 2,
    DummySecurityIK = 3,
    DummyFreeCode3 = 4,
    DummyFreeCode4 = 5,
};

//public string? DummyFreeCode1 { get; set; }
//public int? DummySecurityIK { get => DummyFreeCode4 + securityIK; set => securityIK = value; }
//public string? DummyPortfolio { get => DummyFreeCode1 + "_" + DummyFreeCode2 + "_" + portfolio; set => portfolio = value; }
//public string? DummyFreeCode2 { get; set; }
//public int? DummyHoldingIK { get => DummySecurityIK + DummyPositionIK + DummyFreeCode3 + DummyFreeCode4 + holdingIK; set => holdingIK = value; }
//public string? DummyCurrency { get; set; }
//public DummyInstrumentType? DummyInstrumentType { get; set; }
//public int DummyFreeCode3 { get; set; }
//public int? DummyPositionIK { get => DummyFreeCode3 + positionIK; set => positionIK = value; }
//public int DummyFreeCode4 { get; set; }

var aggregationPositionMaster = new Dictionary<AggregationLevel, KeyValuePair<string, string?>?>() {
    {AggregationLevel.Currency, new KeyValuePair<string,string?>("DummyCurrency","EUR")},
    {AggregationLevel.Holding,  new KeyValuePair<string,string?>("DummyHoldingIK", "24")},
    {AggregationLevel.InstrumentType, new KeyValuePair<string,string?>("DummyInstrumentType","Bond")},
    {AggregationLevel.Position, new KeyValuePair<string,string?>("DummyPositionIK", "6")},
    {AggregationLevel.Portfolio, new KeyValuePair<string,string?>("DummyPortfolio", "AggPosFreeCode1_AggPosFreeCode2_AggPosPortfolio")},
    {AggregationLevel.Security, new KeyValuePair<string,string?>("DummySecurityIK","8")},
    {AggregationLevel.Top, default},
    {AggregationLevel.None, default},
};




var mettaData = new DummyMetaData()
{
    DummyFreeCode1 = "MetaDataFreeCode1",
    DummyFreeCode2 = "MetaDataFreeCode2",
    DummySecurityIK = 3333,
    DummyPositionIK = 2222,
    DummyPortfolio = "MetaDataPortfolio"
};

var mettaDataMaster = new Dictionary<AggregationLevel, KeyValuePair<string, string?>?>() {
    {AggregationLevel.Currency, default},
    {AggregationLevel.Holding,  default},
    {AggregationLevel.InstrumentType, new KeyValuePair<string,string?>("DummyInstrumentType",default)},
    {AggregationLevel.Position, new KeyValuePair<string,string?>("DummyPositionIK","2222")},
    {AggregationLevel.Portfolio, new KeyValuePair <string,string?>("DummyPortfolio","MetaDataPortfolio")},
    {AggregationLevel.Security, new KeyValuePair <string,string?>("DummySecurityIK","3333")},
    {AggregationLevel.Top, default},
    {AggregationLevel.None, default},
};
var selector = new KeyPropertySelector();

//Act
List<bool> l1 = new();
foreach (var item in Enum.GetValues<AggregationLevel>())
{
    KeyValuePair<string, string?>? result = selector.GetPropertyWithAggregationLevel<DummyAggregatPosition>(aggregationPosition, item);
    if (result == null)
    {
        Console.WriteLine($"Null for {item.ToString()}");
    }
    else
    {
        Console.WriteLine($"{result.Value.Key} - {result.Value.Value}");
    }
    if(!aggregationPositionMaster[item].Equals(result))
    {
        Console.WriteLine($"Deviation for {item} - Actual={aggregationPositionMaster[item]},  Result={result}");
    }
    result = selector.GetPropertyWithAggregationLevel<IDummyAggregationPosition>(aggregationPosition, item);
    if (result == null)
    {
        Console.WriteLine($"Null for {item.ToString()}");
    }
    else
    {
        Console.WriteLine($"{result.Value.Key} - {result.Value.Value}");
    }
    if (!aggregationPositionMaster[item].Equals(result))
    {
        Console.WriteLine($"Deviation for {item} - Actual={aggregationPositionMaster[item]},  Result={result}");
    }
    result = selector.GetPropertyWithAggregationLevel<IDdummyMetaData>(aggregationPosition, item);
    if (result == null)
    {
        Console.WriteLine($"Null for {item.ToString()}");
    }
    else
    {
        Console.WriteLine($"{result.Value.Key} - {result.Value.Value}");
    }
    if (!aggregationPositionMaster[item].Equals(result))
    {
        Console.WriteLine($"Deviation for {item} - Actual={aggregationPositionMaster[item]},  Result={result}");
    }

}


Console.WriteLine("Hellow World");



internal class DummyAggregatPosition : IDummyAggregationPosition
{
    private int? holdingIK;
    private int? securityIK;
    private int? positionIK;
    private string? portfolio;

    public string? DummyFreeCode1 { get; set; }
    public int? DummySecurityIK { get => DummyFreeCode4 + securityIK; set => securityIK = value; }
    public string? DummyPortfolio { get => DummyFreeCode1 +"_" + DummyFreeCode2 + "_" + portfolio; set => portfolio = value; }
    public string? DummyFreeCode2 { get; set; }
    public int? DummyHoldingIK { get => DummySecurityIK + DummyPositionIK + DummyFreeCode3 + DummyFreeCode4 + holdingIK; set => holdingIK = value; }
    public string? DummyCurrency { get; set; }
    public DummyInstrumentType? DummyInstrumentType { get; set; }
    public int DummyFreeCode3 { get; set; }
    public int? DummyPositionIK { get => DummyFreeCode3 + positionIK; set => positionIK = value; }
    public int DummyFreeCode4 { get; set; }
}


internal interface IDummyAggregationPosition : IDdummyMetaData
{
    [AggregationLevel(AggregationLevel.Holding)]
    public int? DummyHoldingIK { get; set; }

    [AggregationLevel(AggregationLevel.Currency)]
    public string? DummyCurrency { get; set; }

    public int DummyFreeCode3 { get; set; }
    public int DummyFreeCode4 { get; set; }
}
internal class DummyMetaData : IDdummyMetaData
{
    public int? DummyPositionIK { get; set; }
    public int? DummySecurityIK { get; set; }
    public DummyInstrumentType? DummyInstrumentType { get; set; }
    public string? DummyPortfolio { get; set; }
    public string? DummyFreeCode1 { get; set; }
    public string? DummyFreeCode2 { get; set; }
}


internal interface IDdummyMetaData : IMetaData
{
    [AggregationLevel(AggregationLevel.Position)]
    public int? DummyPositionIK { get; set; }

    [AggregationLevel(AggregationLevel.Security)]
    public int? DummySecurityIK { get; set; }

    [AggregationLevel(AggregationLevel.Portfolio)]
    public string? DummyPortfolio { get; set; }

    [AggregationLevel(AggregationLevel.InstrumentType)]
    public DummyInstrumentType? DummyInstrumentType { get; set; }

    public string? DummyFreeCode1 { get; set; }
    public string? DummyFreeCode2 { get; set; }
}

















//var hostBuilder = Host.CreateDefaultBuilder(args);


//hostBuilder.ConfigureServices(services =>
//{
//    services.AddAggregationService<AggregationPosition, double>();
//    services.AddTransient<IAnalytics<AggregationPosition, double>, MonteCarloAnalytics>();
//    services.AddHostedService<AggregationApplication>();
//});

//var host = hostBuilder.Build();
//host.Start();

//var obj1 = new MyClass()
//{
//    PositionIK = 1,
//    SecurityIK = 2,
//    HoldingIK = 3,
//    Currency = "EUR",
//    Portfolio = "Portfolio1",
//    FreeCode1 = "FreeCode1",
//    FreeCode2 = "FreeCode2",
//    SomeValue = "SomeValue+"
//};

//var obj2 = new MyMyClass()
//{
//    PositionIK = 41,
//    SecurityIK = 25,
//    HoldingIK = 33,
//    Currency = "EURss",
//    Portfolio = "Portfolio1ss",
//    FreeCode1 = "FreeCode1ss",
//    FreeCode2 = "FreeCode2ss",
//    SomeValue = "NotSomeValue+"
//};

//var selector = new DefaultKeyPropertySelector();

//foreach(var level in new[] {AggregationLevel.Currency, AggregationLevel.Position, AggregationLevel.Security, AggregationLevel.Holding, AggregationLevel.Portfolio })
//{
//    var x = selector.GetPropertyWithAggregationLevel<IMetaData>(obj1, level);
//    Console.WriteLine($"Property {x.Key} with Value = {x.Value}");
//}
//Console.WriteLine();
//foreach (var level in new[] { AggregationLevel.Currency, AggregationLevel.Position, AggregationLevel.Security, AggregationLevel.Holding, AggregationLevel.Portfolio })
//{
//    var x = selector.GetPropertyWithAggregationLevel<IMetaData>(obj2, level);
//    Console.WriteLine($"Property {x.Key} with Value = {x.Value}");
//}
//Console.WriteLine();
//foreach (var level in new[] { AggregationLevel.Currency, AggregationLevel.Position, AggregationLevel.Security, AggregationLevel.Holding, AggregationLevel.Portfolio })
//{
//    var x = selector.GetPropertyWithAggregationLevel<IMetaData>(obj1, level);
//    Console.WriteLine($"Property {x.Key} with Value = {x.Value}");
//}


//Console.WriteLine("Hellow World");




