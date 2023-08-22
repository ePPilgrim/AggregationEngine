using SimCorp.AggregationEngine.Core;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace AggregationEngine.MonteCarlo;

public class MonteCarloAnalytics : IAnalytics<AggregationPosition, double>
{
    private readonly IAsyncAggrecationCalculation<DefaultOrderedKey, DefaultUnorderedKey, AggregationPosition, double> aggregationCalculation;

    

    public MonteCarloAnalytics(IAggregationCalculationBuilder<DefaultOrderedKey, DefaultUnorderedKey, AggregationPosition, double> calculationBuilder)
    {
        aggregationCalculation = calculationBuilder.Build(calculator, accumulator);
    }
    public async Task<AggregationPosition> AccumulateForSingleNodeAsync(DefaultOrderedKey nodeKey, CancellationToken token)
    {
        //return await aggregationCalculation.AccumulateForSingleNodeAsync(nodeKey, token);
        return aggregationCalculation.AccumulateForSingleNodeAsync(nodeKey, token).Result;
    }

    public async Task<IDictionary<DefaultUnorderedKey, double>> CalculateSingleNodeAsync(DefaultOrderedKey nodeKey, IDictionary<DefaultUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        return await aggregationCalculation.CalculateSingleNodeAsync(nodeKey, parameters, token);
    }

    public async Task<IDictionary<KeyValuePair<DefaultOrderedKey, DefaultUnorderedKey>, double>> CalculateSubTreeAsync(DefaultOrderedKey rootNodeKey, IDictionary<DefaultUnorderedKey, IParameters> parameters, CancellationToken token)
    {
        return await aggregationCalculation.CalculateSubTreeAsync(rootNodeKey, parameters, token);
    }

    public IDictionary<DefaultUnorderedKey, IMetaData> GetAllLeaves()
    {
        return aggregationCalculation.GetAllLeaves();
    }

    public async Task RemoveAsync(IEnumerable<DefaultUnorderedKey> keys, CancellationToken token)
    {
        await aggregationCalculation.RemoveAsync(keys, token);
    }

    public void SetAggregationStructure(IEnumerable<AggregationLevel> aggregationStructure)
    {
       aggregationCalculation.SetAggregationStructure(aggregationStructure);
    }

    public async Task UpdateOrAddAsync(IEnumerable<AggregationPosition> positions, CancellationToken token)
    {
        await aggregationCalculation.UpdateOrAddAsync(positions, token);
    }

    private async Task<double> calculator(AggregationPosition aggregationPosition, IParameters parameters, CancellationToken token)
    {
        var scenarios = aggregationPosition.Scenarious;
        if(scenarios == null)
        {
            throw new Exception("Scenario array could not be null!");
        }
        return await Task.FromResult(scenarios[3]);
    }

    private async Task<AggregationPosition> accumulator(IEnumerable<AggregationPosition> aggregationPositions, CancellationToken token)
    {
        var fvec = aggregationPositions.First();
        var res = new AggregationPosition(fvec);
        foreach(var position in aggregationPositions.Skip(1))
        {
            res.DoAdditionOperation(position);
        }
        return await Task.FromResult(res);
    }
}
