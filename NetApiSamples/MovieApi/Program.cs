using CommonLibrary;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using MovieApi.HealthChecks;
using System.Net.Http.Headers;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ITMDBService, TMDBService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(TMDBService.BaseUrl);
    var test = configuration[GlobalConstants.TMDBApiKey];
    client.DefaultRequestHeaders.Authorization 
            = new AuthenticationHeaderValue("Bearer", configuration[GlobalConstants.TMDBApiKey]);   
});

// Caching
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITMDBCacheService, TMDBCacheService>();

// Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Rate Limiting
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

builder.Services.AddControllers();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<TMDBHealthCheck>("TMDBService");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

// Rate Limiting
app.UseRateLimiter();

// Compression
app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseAuthorization();

// Health Check Route
app.MapHealthChecks("/health");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    // WriteResponse is a delegate used to customize the health check response.
//    ResponseWriter = (httpContext, result) => HealthChecksHelper.WriteResponse(httpContext, result)
});

app.MapControllers().RequireRateLimiting("fixed");
//app.MapControllers().RequireAuthorization();
app.Run();
