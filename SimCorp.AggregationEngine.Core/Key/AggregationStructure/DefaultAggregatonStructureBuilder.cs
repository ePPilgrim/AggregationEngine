﻿using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.AggregationStructure;

public class DefaultAggregatonStructureBuilder : IAggregationStructureBuilder
{
    public IAggregationStructure BuildEmptyAggregationStructure()
    {
        return BuildFromSequence(new List<AggregationLevel>());
    }

    public IAggregationStructure BuildFromSequence(IEnumerable<AggregationLevel> uniqueOrderedSequenceOfAggregationLevels)
    {
        if(uniqueOrderedSequenceOfAggregationLevels == null)
        {
            throw new ArgumentNullException(nameof(uniqueOrderedSequenceOfAggregationLevels));
        }
        if(uniqueOrderedSequenceOfAggregationLevels.Any(x => x == AggregationLevel.None))
        {
            throw new ArgumentException("Input sequence for aggregation structure contains invalid None value.");
        }
        if(uniqueOrderedSequenceOfAggregationLevels.Distinct().Count() != uniqueOrderedSequenceOfAggregationLevels.Count()) 
        {
            throw new ArgumentException("Input sequence for aggregation structure must be unique.");
        }
        return new DefaultAggregationStructure(new Stack<AggregationLevel>(uniqueOrderedSequenceOfAggregationLevels), this);
    }
}
