namespace SimCorp.AggregationEngine.Core.Key.Common;

public class KeyToStringHelper : IKeyToStringHelper
{
    private const string OrderedKeyPrefix = "Ordered";
    private const string UnorderedKeyPrefix = "Unordered";
    private const string PrefixKeySeparator = "-";
    private const string KeyValueSeparator = ":";
    private const string LeftEnclosure = "{";
    private const string RightEnclosure = "}";
    private const string ItemsSeparator = ",";

    public string OrderKeyToStringKey(string[] keysStringRepresentation, string[] valuesStringRepresentation)
    {
        argumentValidation(keysStringRepresentation, valuesStringRepresentation);
        if (keysStringRepresentation.Length == 0) return string.Empty;
        var res = concatenateKeyValuePairs(keysStringRepresentation, valuesStringRepresentation);
        return OrderedKeyPrefix + PrefixKeySeparator + res;
    }
    public string UnorderKeyToStringKey(string[] keysStringRepresentation, string[] valuesStringRepresentation)
    {
        argumentValidation(keysStringRepresentation, valuesStringRepresentation);
        if (keysStringRepresentation.Length == 0) return string.Empty;
        var res = concatenateKeyValuePairs(keysStringRepresentation, valuesStringRepresentation);
        return UnorderedKeyPrefix + PrefixKeySeparator + res;
    }
    private void argumentValidation(string[] keysStringRepresentation, string[] valuesStringRepresentation)
    {
        if (keysStringRepresentation.Length != valuesStringRepresentation.Length){
            throw new ArgumentException($"Numbers of keys {keysStringRepresentation.Length} and values {valuesStringRepresentation.Length} are not the same.");
        }
    }
    private string concatenateKeyValuePairs(string[] keysStringRepresentation, string[] valuesStringRepresentation)
    {
        var items = keysStringRepresentation.Zip(valuesStringRepresentation).Select(x => LeftEnclosure + x.First + KeyValueSeparator + x.Second + RightEnclosure);
        return LeftEnclosure + string.Join(ItemsSeparator, items) + RightEnclosure;
    }
}
