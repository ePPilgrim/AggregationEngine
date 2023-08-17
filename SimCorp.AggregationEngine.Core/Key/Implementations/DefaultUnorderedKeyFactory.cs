namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultUnorderedKeyFactory : IUnorderedKeyFactory<DefaultUnorderedKey>
{
    private readonly IKeyPropertySelector keyPropertySelector;
    private readonly IKeyToStringHelper keyToStringHelper;
    public DefaultUnorderedKeyFactory(IKeyPropertySelector keyPropertySelector,
                                        IKeyToStringHelper keyToStringHelper)
    {
        this.keyPropertySelector = keyPropertySelector ?? throw new ArgumentNullException(nameof(keyPropertySelector));
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
    }
    public IUnorderedKeyBuilder<DefaultUnorderedKey> CreateUnorderedKeyBuilder()
    {
        return new DefaultUnorderedKeyBuilder(keyPropertySelector, keyToStringHelper);
    }
}
