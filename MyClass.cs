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

/*
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
        var subbuilder = keyFactory.CreateOrderedKeyBuilder(substruct);
        var subOKey = keyFactory.CreateOrderedKeyBuilder(substruct).Build(positions[0]);
        Console.WriteLine($"OrderedSubKey - {subOKey.ToUniqueString()}");

        var paramDictionary = new Dictionary<DefaultUnorderedKey, IParameters>();
        paramDictionary[paramKey] = param;

        var res = analytics.UpdateOrAddAsync(positions, CancellationToken.None);
        res.Wait();
        var accumulatedVectors = analytics.GetAllLeaves().Where(x => subOKey.IsPrefixOf(keybuilder_ordered.Build(x.Value))).ToList();
        var b1 = analytics.GetAllLeaves().Select(x => x.Value).ToList();
        var b2 = b1.GroupBy(x => subbuilder.Build(x)).ToDictionary(x => x.Key, x => x.ToList());    


        int ii = 0;
        //foreach (var leaf in analytics.GetAllLeaves())
        //{
        //    var orkey = keybuilder_ordered.Build(leaf.Value);
        //    Console.WriteLine($"Position{ii++}, UnorderKey - {keybuilder_unordered.BuildForPositions(leaf.Value).ToUniqueString()}, OrderedKey - {orkey.ToUniqueString()} ;");
        //    Console.WriteLine($"Is superset of node - {subOKey.IsPrefixOf(orkey)}");
        //}
        foreach (var level in aggregationStructure.Reverse())
        {
            var subStructure = aggregationStructure.GetSubStructureAt(level);
            Console.WriteLine($"SubStructure - {subStructure}");
            var keybuilder = keyFactory.CreateOrderedKeyBuilder(subStructure);
            int iii = 0;
            foreach (var leaf in analytics.GetAllLeaves())
            {
                var key = keybuilder_ordered.Build(leaf.Value);
                var subkey = key.GetSubKey(subStructure);
                Console.WriteLine($"Position{iii++}, OrderedKey1 - {keybuilder.Build(leaf.Value).ToUniqueString()},  OrderedKey2 - {subkey} ;");

            }
        }


        var vecRes = analytics.AccumulateForSingleNodeAsync(subOKey, CancellationToken.None).Result;

       var calcReses = analytics.CalculateSingleNodeAsync(subOKey, paramDictionary, CancellationToken.None).Result;

       var calcSubTree = analytics.CalculateSubTreeAsync(subOKey, paramDictionary, CancellationToken.None).Result;

      
        //foreach(var calcRes in calcReses)
        //{
        //    Console.WriteLine($"Result {calcRes.Key.ToUniqueString} = {calcRes.Value}");
        //}

        Console.WriteLine("Hellow World");

    }
 */
