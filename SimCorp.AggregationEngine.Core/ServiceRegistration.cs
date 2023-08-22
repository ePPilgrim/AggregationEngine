using Microsoft.Extensions.DependencyInjection;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;

namespace SimCorp.AggregationEngine.Core;

public static class ServiceRegistration
{ 
    public static IServiceCollection AddAggregationService<TVector, TResult>(this IServiceCollection services) where TVector : IAggregationPosition
    {
        services.AddSingleton<IKeyPropertySelector, DefaultKeyPropertySelector>();
        services.AddSingleton<IKeyToStringHelper, DefaultKeyToStringHelper>();
        services.AddSingleton<IAsyncExternalDataAllocator<TVector>, DummyAsyncExternalDataAllocator<TVector>>();
        services.AddSingleton<IAsyncExternalDataAllocator<TResult>, DummyAsyncExternalDataAllocator<TResult>>();
        services.AddTransient<IKeyFactory<DefaultOrderedKey, DefaultUnorderedKey>, DefaultKeyFactory>();
        services.AddTransient<IDataLayerFactory<TVector>, DefaultDataLayerFactory<TVector>>();
        services.AddTransient<IDataLayerFactory<TResult>, DefaultDataLayerFactory<TResult>>();
        services.AddTransient<IPositionDataLayerFactory<TVector>, DefaultPositionDataLayerFactory<TVector>>();
        services.AddTransient<IOptimizationPolicyInternal, DefaultOptimizationPolicyInternal>();
        services.AddTransient<IAggregationCalculationFactoryInternal<DefaultOrderedKey, DefaultUnorderedKey, TVector, TResult>, DefaultAggregationCalculationFactoryInternal<DefaultOrderedKey, DefaultUnorderedKey, TVector, TResult>>();
        services.AddTransient<IAggregationCalculationBuilder<DefaultOrderedKey, DefaultUnorderedKey, TVector, TResult>>(x => new DefaultAggregationCalculationBuilder<DefaultOrderedKey, DefaultUnorderedKey, TVector, TResult>(x));
        return services;
    }
}
