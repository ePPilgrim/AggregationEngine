namespace SimCorp.AggregationEngine.Core.Key.Common;

public interface IKey : IEquatable<IKey>
{
    string ToUniqueString();
}
