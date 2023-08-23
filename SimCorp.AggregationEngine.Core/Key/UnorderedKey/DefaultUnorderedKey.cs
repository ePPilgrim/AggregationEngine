namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public class DefaultUnorderedKey : IKey
{
    private readonly IKeyToStringHelper keyToStringHelper;
    private readonly string key;

    public DefaultUnorderedKey(IKeyToStringHelper keyToStringHelper, IReadOnlyDictionary<string, string?> keyValuePairs)
    {
        this.keyToStringHelper = keyToStringHelper;
        if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));
        key = buildKey(keyValuePairs);
    }

    public bool Equals(IKey? other)
    {
        if (other == null) return false;
        return other.ToUniqueString() == ToUniqueString();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IKey);
    }

    public string ToUniqueString()
    {
        return key;
    }

    public override string ToString()
    {
        return key;
    }

    private string buildKey(IReadOnlyDictionary<string, string?> keyValuePairs)
    {
        var orderedSequence = keyValuePairs.Where(x => x.Value != null).OrderByDescending(x => x.Key);
        return keyToStringHelper.UnorderKeyToStringKey(orderedSequence.Select(x => x.Key).ToArray(), orderedSequence.Select(x => x.Value!).ToArray());
    }
}
