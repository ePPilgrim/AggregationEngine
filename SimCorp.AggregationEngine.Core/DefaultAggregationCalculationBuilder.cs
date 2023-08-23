using Microsoft.Extensions.DependencyInjection;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.ExternalAllocator;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Internal.DataLayer.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Internal.DataLayer;
using SimCorp.AggregationEngine.Core.Internal.DefaultImplementation;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;
using SimCorp.AggregationEngine.Core.Key.UnorderedKey;
using SimCorp.AggregationEngine.Core.Key.Common;

namespace SimCorp.AggregationEngine.Core;

public class DefaultAggregationCalculationBuilder<TOrderedKey, TUnorderedKey, TVector, TResult> : IAggregationCalculationBuilder<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                                                                                                where TUnorderedKey : IKey
                                                                                                                                                                                where TVector : IAggregationPosition
{
    private readonly IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> internalsFactory;
    private readonly IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;

    public DefaultAggregationCalculationBuilder(IServiceProvider serviceProvider)
    {
        //var t1 = serviceProvider.GetRequiredService<IKeyPropertySelector>();
        //var t2 = serviceProvider.GetRequiredService<IKeyToStringHelper>();
        //var t3 = serviceProvider.GetRequiredService<IAsyncExternalDataAllocator<TVector>>();
        //var t4 = serviceProvider.GetRequiredService<IAsyncExternalDataAllocator<TResult>>();
        //var t5 = serviceProvider.GetRequiredService<IKeyFactory<DefaultOrderedKey, DefaultUnorderedKey>>();
        //var t6 = serviceProvider.GetRequiredService<IPositionDataLayerFactory<TVector>>();
        //var t7 = serviceProvider.GetRequiredService<IDataLayerFactory<TResult>>();
        //var t8 = serviceProvider.GetRequiredService<IOptimizationPolicyInternal>();


        keyFactory = serviceProvider.GetRequiredService<IKeyFactory<TOrderedKey, TUnorderedKey>>();
        if (keyFactory == null) throw new ArgumentNullException(nameof(keyFactory));
        internalsFactory = serviceProvider.GetRequiredService<IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>>();
        if (internalsFactory == null) throw new ArgumentNullException(nameof(internalsFactory));

    }
    public IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> Build(Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator, Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator)
    {
        return new DefaultAsyncAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult>(internalsFactory, keyFactory, calculator, accumulator); 
    }
}
