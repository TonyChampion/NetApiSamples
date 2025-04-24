using Asp.Versioning;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using CommonLibrary;
using CommonLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MovieApiVersioning;
using MovieApiVersioning.Endpoints;
using System.Net.Http.Headers;

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


/*builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});*/

builder.Services.AddApiVersioning(
                    options =>
                    {
                        // reporting api versions will return the headers
                        // "api-supported-versions" and "api-deprecated-versions"
                        options.DefaultApiVersion = new ApiVersion(1);
                        options.ReportApiVersions = true;
                        options.ApiVersionReader = ApiVersionReader.Combine(
                                    new UrlSegmentApiVersionReader(),
                                    new QueryStringApiVersionReader("api-version"),
                                    new HeaderApiVersionReader("X-Version"),
                                    new MediaTypeApiVersionReader("x-version"));

                    })
                    .AddApiExplorer(
                    options =>
                    {
                        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                        // note: the specified format code will format the version as "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";

                        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                        // can also be used to control the format of the API version in route templates
                        options.SubstituteApiVersionInUrl = true;
                    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

var app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(1)
    .HasApiVersion(2)
    .ReportApiVersions()
    .Build();

RouteGroupBuilder routeGroupBuilder = app.MapGroup("/api/v{apiVersion:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

routeGroupBuilder.MapMovieV1Endpoints();
routeGroupBuilder.MapMovieV2Endpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/openapi/{description.GroupName}.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
        options.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();

app.Run();
