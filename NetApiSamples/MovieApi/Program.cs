using CommonLibrary;
using CommonLibrary.Services;
using Microsoft.AspNetCore.RateLimiting;
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
/*builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});*/

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Rate Limiting
app.UseRateLimiter();

// Compression
//app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting("fixed");

app.Run();
