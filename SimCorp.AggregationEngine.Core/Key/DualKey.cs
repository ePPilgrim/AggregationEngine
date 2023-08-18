namespace SimCorp.AggregationEngine.Core.Key;

public struct DualKey<TKey1, TKey2> : IKey where TKey1 : IKey
                                            where TKey2 : IKey

{
    public TKey1 First { get; }
    public TKey2 Second { get; }

    public DualKey(TKey1 key1, TKey2 key2)
    {
        First = key1;
        Second = key2;
    }
    public bool Equals(IKey? other)
    {
        throw new NotImplementedException();
    }

    public string ToUniqueString()
    {
        return "Unordered" + First.ToUniqueString() + Second.ToUniqueString();
    }
}
