using System.Collections;

namespace SimCorp.AggregationEngine.Core.Key;

public interface IKey :IEquatable<IKey> 
{
    string ToUniqueString();
}
