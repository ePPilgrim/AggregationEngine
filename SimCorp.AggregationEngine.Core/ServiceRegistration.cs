using Microsoft.Extensions.DependencyInjection;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.Common;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core;

public static class ServiceRegistration
{ 
    public static IServiceCollection AddAggregationService<TVector, TResult>(this IServiceCollection services) where TVector : IAggregationPosition
    {
        services.AddSingleton<IKeyPropertySelector, KeyPropertySelector>();
        services.AddSingleton<IKeyToStringHelper, KeyToStringHelper>();
        services.AddSingleton<IAsyncExternalDataAllocator<TVector>, DummyAsyncExternalDataAllocator<TVector>>();
        services.AddSingleton<IAsyncExternalDataAllocator<TResult>, DummyAsyncExternalDataAllocator<TResult>>();
        services.AddTransient<IKeyFactory<OrderedKey, UnorderedKey>, KeyFactory>();
        services.AddTransient<IDataLayerFactory<TVector>, DefaultDataLayerFactory<TVector>>();
        services.AddTransient<IDataLayerFactory<TResult>, DefaultDataLayerFactory<TResult>>();
        services.AddTransient<IPositionDataLayerFactory<TVector>, DefaultPositionDataLayerFactory<TVector>>();
        services.AddTransient<IOptimizationPolicyInternal, DefaultOptimizationPolicyInternal>();
        services.AddTransient<IAggregationCalculationFactoryInternal<OrderedKey, UnorderedKey, TVector, TResult>, DefaultAggregationCalculationFactoryInternal<OrderedKey, UnorderedKey, TVector, TResult>>();
        services.AddTransient<IAggregationCalculationBuilder<OrderedKey, UnorderedKey, TVector, TResult>>(x => new DefaultAggregationCalculationBuilder<OrderedKey, UnorderedKey, TVector, TResult>(x));
        return services;
    }
}
