using SimCorp.AggregationEngine.Core.Domain;

namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultOrderedKeyFactory : IOrderedKeyFactory<DefaultOrderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    private readonly IAggregationStructureFactory aggregationStructureFactory;
    public DefaultOrderedKeyFactory(IKeyPropertySelector keyPropertySelector,
                                    IKeyToStringHelper keyToStringHelper,
                                    IAggregationStructureFactory aggregationStructureFactory)
    {
        this.keyPropertySelector = keyPropertySelector ?? throw new ArgumentNullException(nameof(keyPropertySelector));
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
        this.aggregationStructureFactory = aggregationStructureFactory ?? throw new ArgumentNullException(nameof(aggregationStructureFactory));
    }
    public IAggregationStructure CreateAggregationStructure(IEnumerable<AggregationLevel> uniqueOrderedSequenceOfAggregationLevels)
    {
        return aggregationStructureFactory.CreateAggregationStructure(uniqueOrderedSequenceOfAggregationLevels);
    }

    public IAggregationStructure CreateEmptyAggregationStructure()
    {
        return aggregationStructureFactory.CreateEmptyAggregationStructure();
    }

    public IOrderedKeyBuilder<DefaultOrderedKey> CreateOrderedKeyBuilder(IAggregationStructure aggregationStructure)
    {
        return new DefaultOrderedKeyBuilder(keyPropertySelector, keyToStringHelper,this,aggregationStructure);
    }
}
