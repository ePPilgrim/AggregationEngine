using AggregationEngine.MonteCarlo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimCorp.AggregationEngine.Core;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;
using StackExchange.Redis;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;

namespace AggregationEngine;

internal class AggregationApplication : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    public AggregationApplication(IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        hostApplicationLifetime.ApplicationStopped.Register(OnStopped);
        return Task.CompletedTask;
    }

    private async void OnStarted()
    {
        var analytics = serviceProvider.GetRequiredService<IAnalytics<AggregationPosition, double>>();
        var aggrStructure = new List<AggregationLevel> {    AggregationLevel.Top, AggregationLevel.Portfolio, 
                                                            AggregationLevel.Currency, AggregationLevel.Holding,
                                                            AggregationLevel.Security, AggregationLevel.Position};
        var positions = GeneratePositions.GetPositionsRandom();

        var pos1 = positions[0];
        var t1 = JsonSerializer.SerializeToUtf8Bytes(pos1);
        var t2 = JsonSerializer.Deserialize<AggregationPosition>(t1);


        var keyFactory = serviceProvider.GetRequiredService<IKeyFactory<DefaultOrderedKey, DefaultUnorderedKey>>();
        var aggrStructureBuilder = keyFactory.CreateAggregationStructureBuilder();
        var aggregationStructure = aggrStructureBuilder.BuildFromSequence(aggrStructure);
        var keybuilder_unordered = keyFactory.CreateUnorderedKeyBuilder();
        var keybuilder_ordered = keyFactory.CreateOrderedKeyBuilder(aggregationStructure);
        //foreach(var i in Enumerable.Range(0, positions.Count))
        //{
        //    var pos = positions[i];
        //    Console.WriteLine($"Position{i}, UnorderKey - {keybuilder_unordered.BuildForPositions(pos).ToUniqueString()}, OrderedKey - {keybuilder_ordered.Build(pos).ToUniqueString()} ;");
        //}

        analytics.SetAggregationStructure(aggrStructure);
        var param = new MonteCarloParameters() { ConfidenceLevel = 0.99 };
        var paramKey = keybuilder_unordered.BuildForParameters(param);
        Console.WriteLine($"Key for param - {paramKey.ToUniqueString()}");

        var substruct = aggrStructureBuilder.BuildFromSequence(new[] { AggregationLevel.Top });
        var subOKey = keyFactory.CreateOrderedKeyBuilder(substruct).Build(positions[0]);
        Console.WriteLine($"OrderedSubKey - {subOKey.ToUniqueString()}");

        var paramDictionary = new Dictionary<DefaultUnorderedKey, IParameters>();
        paramDictionary[paramKey] = param;

        var res = analytics.UpdateOrAddAsync(positions, CancellationToken.None);
        res.Wait();
        var accumulatedVectors = analytics.GetAllLeaves().Where(x => subOKey.IsPrefixOf(keybuilder_ordered.Build(x.Value))).ToList();
        int ii = 0;
        //foreach (var leaf in analytics.GetAllLeaves())
        //{
        //    var orkey = keybuilder_ordered.Build(leaf.Value);
        //    Console.WriteLine($"Position{ii++}, UnorderKey - {keybuilder_unordered.BuildForPositions(leaf.Value).ToUniqueString()}, OrderedKey - {orkey.ToUniqueString()} ;");
        //    Console.WriteLine($"Is superset of node - {subOKey.IsPrefixOf(orkey)}");
        //}
        //foreach (var level in aggregationStructure.Reverse())
        //{
        //    var subStructure = aggregationStructure.GetSubStructureAt(level);
        //    Console.WriteLine($"SubStructure - {subStructure}");
        //    var keybuilder = keyFactory.CreateOrderedKeyBuilder(subStructure);
        //    int iii = 0;
        //    foreach (var leaf in analytics.GetAllLeaves())
        //    {
        //        var key = keybuilder_ordered.Build(leaf.Value);
        //        var subkey = key.GetSubKey(subStructure);
        //        Console.WriteLine($"Position{iii++}, OrderedKey1 - {keybuilder.Build(leaf.Value).ToUniqueString()},  OrderedKey2 - {subkey} ;");

        //    }
        //}


            //var vecRes = analytics.AccumulateForSingleNodeAsync(subOKey, CancellationToken.None).Result;

            //var calcReses = analytics.CalculateSingleNodeAsync(subOKey, paramDictionary, CancellationToken.None).Result;

       var calcSubTree = analytics.CalculateSubTreeAsync(subOKey, paramDictionary, CancellationToken.None).Result;

      
        //foreach(var calcRes in calcReses)
        //{
        //    Console.WriteLine($"Result {calcRes.Key.ToUniqueString} = {calcRes.Value}");
        //}

        Console.WriteLine("Hellow World");

    }

    private void OnStopped()
    {
       Console.WriteLine("Application has finished!!!!");
    }
}
