namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IKeyToStringConvertor<in T> 
{
    string KeyToString(T value);
}
