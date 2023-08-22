// See https://aka.ms/new-console-template for more information

using AggregationEngine;
using AggregationEngine.MonteCarlo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimCorp.AggregationEngine.Core;

var hostBuilder = Host.CreateDefaultBuilder(args);


hostBuilder.ConfigureServices(services =>
{
    services.AddAggregationService<AggregationPosition, double>();
    services.AddTransient<IAnalytics<AggregationPosition, double>, MonteCarloAnalytics>();
    services.AddHostedService<AggregationApplication>();
});

var host = hostBuilder.Build();
host.Start();





