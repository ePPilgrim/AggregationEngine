// See https://aka.ms/new-console-template for more information

using AggregationEngine;
using AggregationEngine.MonteCarlo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimCorp.AggregationEngine.Core;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;


var hostBuilder = Host.CreateDefaultBuilder(args);


hostBuilder.ConfigureServices(services =>
{
    services.AddAggregationService<AggregationPosition, double>();
    services.AddTransient<IAnalytics<AggregationPosition, double>, MonteCarloAnalytics>();
    services.AddHostedService<AggregationApplication>();
});

var host = hostBuilder.Build();
host.Start();

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




