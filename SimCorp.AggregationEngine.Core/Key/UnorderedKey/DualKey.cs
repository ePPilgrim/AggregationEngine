using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Key.UnorderedKey;

internal class DualKey<TKey1, TKey2> : AbstractKey   where TKey1 : IKey
                                                     where TKey2 : IKey
{
    public TKey1 First { get; }
    public TKey2 Second { get; }

    public DualKey(TKey1 key1, TKey2 key2)
    {
        First = key1;
        Second = key2;
    }
    public override bool Equals(IKey? other)
    {
        var otherDualKey = other as DualKey<TKey1, TKey2>;
        if (otherDualKey == null) return false;
        return First.Equals(otherDualKey.First) && Second.Equals(otherDualKey.Second);
    }

    public override string ToUniqueString() => "Unordered" + First.ToUniqueString() + "_" + Second.ToUniqueString();
}
