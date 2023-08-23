using AggregationEngine.MonteCarlo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimCorp.AggregationEngine.Core;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

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

        var keyFactory = serviceProvider.GetRequiredService<IKeyFactory<OrderedKey, UnorderedKey>>();
        var aggrStructure = new List<AggregationLevel> {    AggregationLevel.Top, AggregationLevel.Portfolio, 
                                                            AggregationLevel.Currency, AggregationLevel.Holding,
                                                            AggregationLevel.Security, AggregationLevel.Position};
        var aggrStructureBuilder = keyFactory.CreateAggregationStructureBuilder();
        var aggregationStructure = aggrStructureBuilder.BuildFromSequence(aggrStructure);

        var positions = GeneratePositions.GetPositionsRandom();

        var keybuilder_unordered = keyFactory.CreateUnorderedKeyBuilder();
        var keybuilder_ordered = keyFactory.CreateOrderedKeyBuilder(aggregationStructure);

        Console.WriteLine($"Aggregation structure: {aggregationStructure}\n\n");
        Console.WriteLine("-----------------------------------------------------------------------------------------\n\n");
        foreach (var i in Enumerable.Range(0, positions.Count))
        {
            var pos = positions[i];
            Console.WriteLine($"Position{i}:");
            Console.WriteLine($"UnorderKey - {keybuilder_unordered.BuildForPositions(pos).ToUniqueString()}");
            Console.WriteLine($"OrderedKey - {keybuilder_ordered.Build(pos).ToUniqueString()}.");
            Console.WriteLine();
        }
        Console.WriteLine("-----------------------------------------------------------------------------------------\n\n");
        analytics.SetAggregationStructure(aggrStructure);
        var param1 = new MonteCarloParameters() { ConfidenceLevel = 0.99 };
        var param2 = new MonteCarloParameters() { ConfidenceLevel = 0.01 };
        var paramDictionary = new Dictionary<UnorderedKey, IParameters>()
        {
            {keybuilder_unordered.BuildForParameters(param1), param1 },
            {keybuilder_unordered.BuildForParameters(param2), param2 },
        };
        Console.WriteLine("Parameters for MC calculation:");
        foreach(var item in paramDictionary)
        {
            Console.WriteLine($"Parameters with Unordered key {item.Key} has confidential level {item.Value};");
        }
        Console.WriteLine("-----------------------------------------------------------------------------------------\n\n");

        var substruct = aggrStructureBuilder.BuildFromSequence(new[] { AggregationLevel.Top });
        var subbuilder = keyFactory.CreateOrderedKeyBuilder(substruct);
        var subOKey = keyFactory.CreateOrderedKeyBuilder(substruct).Build(positions[0]);
       
        var res = analytics.UpdateOrAddAsync(positions, CancellationToken.None);
        res.Wait();

        var vecRes = analytics.AccumulateForSingleNodeAsync(subOKey, CancellationToken.None).Result;

        var calcReses = analytics.CalculateSingleNodeAsync(subOKey, paramDictionary, CancellationToken.None).Result;
        foreach (var calcRes in calcReses)
        {
            Console.WriteLine($"Result {calcRes.Key} = {calcRes.Value}");
        }

        var calcSubTree = analytics.CalculateSubTreeAsync(subOKey, paramDictionary, CancellationToken.None).Result;
        var allKeys = new List<string>();
        foreach (var level in aggregationStructure.Reverse())
        {
            var subStructure = aggregationStructure.GetSubStructureAt(level);
            var keybuilder = keyFactory.CreateOrderedKeyBuilder(subStructure);
            foreach (var leaf in analytics.GetAllLeaves())
            {
                var key = keybuilder_ordered.Build(leaf.Value);
                var subkey = key.GetSubKey(subStructure);
                allKeys.Add(subkey.ToUniqueString());

            }
        }

        int ss = allKeys.Distinct().Count();


        Console.WriteLine("Done!");

    }

    private void OnStopped()
    {
       Console.WriteLine("Application has finished!!!!");
    }
}
