using AggregationEngine.Core.Domain;

namespace AggregationEngine.Core;

public interface IAggregationStructure : IList<AggregationLevel>
{
    AggregationLevel Next(AggregationLevel aggregationLevel);
    AggregationLevel Previous(AggregationLevel aggregationLevel);
    IAggregationStructure GetSubStructure(AggregationLevel from, AggregationLevel to);

}
