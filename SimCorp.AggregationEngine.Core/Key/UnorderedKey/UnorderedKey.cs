using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

public class UnorderedKey : AbstractKey
{
    private readonly IKeyToStringHelper keyToStringHelper;
    private readonly string key;

    public UnorderedKey(IKeyToStringHelper keyToStringHelper, IReadOnlyDictionary<string, string?> keyValuePairs)
    {
        this.keyToStringHelper = keyToStringHelper;
        if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));
        key = buildKey(keyValuePairs);
    }

    public override bool Equals(IKey? other)
    {
        if (other == null) return false;
        return other.ToUniqueString() == ToUniqueString();
    }

    public override string ToUniqueString()
    {
        return key;
    }

    private string buildKey(IReadOnlyDictionary<string, string?> keyValuePairs)
    {
        var orderedSequence = keyValuePairs.Where(x => x.Value != null).OrderByDescending(x => x.Key);
        return keyToStringHelper.UnorderKeyToStringKey(orderedSequence.Select(x => x.Key).ToArray(), orderedSequence.Select(x => x.Value!).ToArray());
    }
}
