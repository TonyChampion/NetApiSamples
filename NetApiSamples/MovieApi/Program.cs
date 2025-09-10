using CommonLibrary;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using MovieApi.Endpoints;
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

// Caching
builder.Services.AddOutputCache();
builder.Services.AddHybridCache();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<TMDBHealthCheck>("TMDBService");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Rate Limiting
app.UseRateLimiter();

// Compression
app.UseResponseCompression();

app.UseHttpsRedirection();

// Health Check Route
app.MapHealthChecks("/health");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    // WriteResponse is a delegate used to customize the health check response.
    // ResponseWriter = (httpContext, result) => HealthChecksHelper.WriteResponse(httpContext, result)
});

// Caching
app.UseOutputCache();

// Rate limiting
//RouteGroupBuilder routeGroupBuilder = app.MapGroup("").RequireRateLimiting("fixed");
//routeGroupBuilder.MapMovieEndpoints();

app.MapMovieEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Swagger");
        options.EnableTryItOutByDefault();
    });
}

app.Run();
