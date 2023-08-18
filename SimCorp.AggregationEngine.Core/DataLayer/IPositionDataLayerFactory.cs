using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Key;

namespace SimCorp.AggregationEngine.Core.DataLayer;

internal interface IPositionDataLayerFactory<TVector> : IDataLayerFactory<TVector> where TVector : IAggregationPosition
{
    IAsyncMapVectorDecoratorInternal<TKey, TVector> Create<TKey>(IAsyncMapInternal<TKey, TVector> dataAllocator) where TKey : IKey;
    IAllocatorWrapperBuilderInternal<TKey, TVector> CreateBuilder<TKey>(IAsyncMapInternal<TKey, TVector> dataAllocator) where TKey : IKey;
}
