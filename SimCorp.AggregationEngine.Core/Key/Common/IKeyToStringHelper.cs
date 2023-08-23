namespace SimCorp.AggregationEngine.Core.Key.Common;

public interface IKeyToStringHelper
{
    string OrderKeyToStringKey(string[] keysStringRepresentation, string[] valuesStringRepresentation);

    string UnorderKeyToStringKey(string[] keysStringRepresentation, string[] valuesStringRepresentation);
}
