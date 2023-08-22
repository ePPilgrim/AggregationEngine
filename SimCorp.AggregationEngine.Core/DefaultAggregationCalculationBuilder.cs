using Microsoft.Extensions.DependencyInjection;
using SimCorp.AggregationEngine.Core.Domain;
using SimCorp.AggregationEngine.Core.Internal;
using SimCorp.AggregationEngine.Core.Key;
using SimCorp.AggregationEngine.Core.Key.OrderedKey;

namespace SimCorp.AggregationEngine.Core;

public class DefaultAggregationCalculationBuilder<TOrderedKey, TUnorderedKey, TVector, TResult> : IAggregationCalculationBuilder<TOrderedKey, TUnorderedKey, TVector, TResult> where TOrderedKey : IOrderedKey<TOrderedKey>
                                                                                                                                                                                where TUnorderedKey : IKey
                                                                                                                                                                                where TVector : IAggregationPosition
{
    private readonly IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult> internalsFactory;
    private readonly IKeyFactory<TOrderedKey, TUnorderedKey> keyFactory;

    public DefaultAggregationCalculationBuilder(IServiceProvider serviceProvider)
    {
        keyFactory = serviceProvider.GetRequiredService<IKeyFactory<TOrderedKey, TUnorderedKey>>();
        if (keyFactory == null) throw new ArgumentNullException(nameof(keyFactory));
        //var oo = serviceProvider.GetRequiredService<IOptimizationPolicyInternal>();
        internalsFactory = serviceProvider.GetRequiredService<IAggregationCalculationFactoryInternal<TOrderedKey, TUnorderedKey, TVector, TResult>>();
        if (internalsFactory == null) throw new ArgumentNullException(nameof(internalsFactory));

    }
    public IAsyncAggrecationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult> Build(Func<TVector, IParameters, CancellationToken, Task<TResult>> calculator, Func<IEnumerable<TVector>, CancellationToken, Task<TVector>> accumulator)
    {
        return new DefaultAsyncAggregationCalculation<TOrderedKey, TUnorderedKey, TVector, TResult>(internalsFactory, keyFactory, calculator, accumulator); 
    }
}
