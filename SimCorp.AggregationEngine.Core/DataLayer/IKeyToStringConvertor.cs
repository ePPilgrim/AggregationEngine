namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IKeyToStringConvertor<in T> where T : class
{
    string KeyToString(T value);
}
