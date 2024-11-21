using AzureWorkflowCommon.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

string serviceBusConnectionString = builder.Configuration["ServiceBusConnection"];
string tableStorageConnectionString = builder.Configuration["TableStorageConnection"];

builder.Services.AddAzureClients(clientsBuilder =>
{
    clientsBuilder.AddServiceBusClient(serviceBusConnectionString)
        .WithName("servicebusClient");

    clientsBuilder.AddTableServiceClient(tableStorageConnectionString)
        .WithName("tableStorageClient");

});

builder.Services.AddSingleton<IServiceBusService, ServiceBusService>();
builder.Services.AddSingleton<ITableStorageService, TableStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Swagger");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
