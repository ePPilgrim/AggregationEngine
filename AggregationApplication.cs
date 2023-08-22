using AggregationEngine.MonteCarlo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimCorp.AggregationEngine.Core;

namespace AggregationEngine;

internal class AggregationApplication : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    public AggregationApplication(IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        hostApplicationLifetime.ApplicationStopped.Register(OnStopped);
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        var analytics = serviceProvider.GetRequiredService<IAnalytics<AggregationPosition, double>>();

        Console.WriteLine("Hellow World");

    }

    private void OnStopped()
    {
       Console.WriteLine("Application has finished!!!!");
    }
}
