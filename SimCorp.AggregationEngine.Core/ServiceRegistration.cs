using Microsoft.Extensions.DependencyInjection;
using SimCorp.AggregationEngine.Core.Domain;
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
    public static IServiceCollection AddInternalServices<TOrderedKey, TUnorderedKey, TVector, TResult>(this IServiceCollection services) where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                                                         where TUnorderedKey : IKey
                                                                                                                                         where TVector : IAggregationPosition
    {
        services.AddTransient<IPositionDataLayerFactory<TVector>, DefaultPositionDataLayerFactory<TVector>>();  
        services.AddTransient<IDataLayerFactory<TResult>, DefaultDataLayerFactory<TResult>>();
        services.AddTransient<IKeyFactory<DefaultOrderedKey, DefaultUnorderedKey>, DefaultKeyFactory>();
        services.AddTransient<IOptimizationPolicyInternal, DefaultOptimizationPolicyInternal>();
        services.AddTransient<IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>, DefaultAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>>();
        return services;
    }

}
