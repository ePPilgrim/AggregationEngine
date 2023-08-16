namespace SimCorp.AggregationEngine.Core.Key.Implementations;

public class DefaultUnorderedKey : IKey
{
    private readonly IKeyToStringHelper keyToStringHelper;
    private readonly string key;

    public DefaultUnorderedKey(IKeyToStringHelper keyToStringHelper, IReadOnlyDictionary<string,string> keyValuePairs)
    {
        this.keyToStringHelper = keyToStringHelper ?? throw new ArgumentNullException(nameof(keyToStringHelper));
        if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));
        key = buildKey(keyValuePairs);
    }

    public bool Equals(IKey? other)
    {
        var otherKey = other as DefaultUnorderedKey;
        if(otherKey == null) return false;
        return otherKey.ToUniqueString() == ToUniqueString();
    }

    public string ToUniqueString()
    {
        return key;
    }

    private string buildKey(IReadOnlyDictionary<string, string> keyValuePairs)
    {
        var orderedSequence = keyValuePairs.OrderByDescending(x => x.Key);
        return keyToStringHelper.UnorderKeyToStringKey(orderedSequence.Select(x => x.Key).ToArray(), orderedSequence.Select(x => x.Value).ToArray());
    }
}
