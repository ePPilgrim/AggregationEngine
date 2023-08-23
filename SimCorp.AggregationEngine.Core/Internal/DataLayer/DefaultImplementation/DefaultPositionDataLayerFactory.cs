using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;

internal class DefaultPositionDataLayerFactory<TVector> : IPositionDataLayerFactory<TVector> where TVector : IAggregationPosition
{
    private readonly IDataLayerFactory<TVector> dataLayerFactory;

    public DefaultPositionDataLayerFactory(IDataLayerFactory<TVector> dataLayerFactory)
    {
        this.dataLayerFactory = dataLayerFactory ?? throw new ArgumentNullException(nameof(dataLayerFactory));
    }
    public IAsyncMapVectorWrapperInternal<TKey, TVector> Create<TKey>(IAsyncMapInternal<TKey, TVector> internalDataAllocator) where TKey : IKey
    {
        return new DefaultAsyncMapVectorWrapperInternal<TKey, TVector>(internalDataAllocator, this);
    }

    public IAsyncMapInternal<TKey, TVector> Create<TKey>() where TKey : IKey
    {
        return dataLayerFactory.Create<TKey>();
    }

    public IAsyncMapInternal<TKey, TVector> Create<TKey>(IAsyncExternalDataAllocator<TVector> externalDataAllocator) where TKey : IKey
    {
        return dataLayerFactory.Create<TKey>(externalDataAllocator);
    }
}
