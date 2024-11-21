using AzureWorkflowCommon.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

string tableStorageConnectionString = builder.Configuration["TableStorageConnection"];

builder.Services.AddAzureClients(clientsBuilder =>
{
    clientsBuilder.AddTableServiceClient(tableStorageConnectionString)
        .WithName("tableStorageClient");

});

builder.Services.AddSingleton<ITableStorageService, TableStorageService>();

builder.Build().Run();
