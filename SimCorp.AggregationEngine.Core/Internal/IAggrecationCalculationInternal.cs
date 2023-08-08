using SimCorp.AggregationEngine.Core.DataCollections;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.Internal;

internal interface IAggrecationCalculationInternal<TOrderedKey, TUnorderedKey, TVector, TResult>    where TOrderedKey : IKey
                                                                                                    where TUnorderedKey : IEqualityComparer<TUnorderedKey>      
                                                                                                    where TVector : struct, IAggregationPosition
{
    void Add(IMap<TUnorderedKey, TVector> positions);
    void Clear();
    void Remove(IEnumerable<TUnorderedKey> keys);
    IMap<TUnorderedKey, TVector> GetAllLeaves();

    IMap<TUnorderedKey, TResult> CalculateSingleNode(TOrderedKey nodeKey, 
                                                     IMap<TUnorderedKey, IParameters> parameters,  
                                                     Func<TVector, IParameters, TResult> calculator,
                                                     Func<IEnumerable<TVector>, TVector> accumulator);
    IMap<TOrderedKey, IMap<TUnorderedKey, TResult>> CalculateSubTree(   TOrderedKey rootNodeKey,
                                                                        IMap<TUnorderedKey, IParameters> parameters, 
                                                                        Func<TVector, IParameters, TResult> calculator, 
                                                                        Func<IEnumerable<TVector>, TVector> accumulator);
    TVector AccumulateForSingleNode(TOrderedKey nodeKey, Func<IEnumerable<TVector>, TVector> accumulator);
}
