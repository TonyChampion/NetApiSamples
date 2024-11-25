using Asp.Versioning;
using Asp.Versioning.Conventions;
using CommonLibrary;
using CommonLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MovieApiVersioning;
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

builder.Services.AddControllers();

builder.Services.AddApiVersioning(
                    options =>
                    {
                        // reporting api versions will return the headers
                        // "api-supported-versions" and "api-deprecated-versions"
                        options.ReportApiVersions = true;
                        options.ApiVersionReader = ApiVersionReader.Combine(
                                    new UrlSegmentApiVersionReader(),
                                    new QueryStringApiVersionReader("api-version"),
                                    new HeaderApiVersionReader("X-Version"),
                                    new MediaTypeApiVersionReader("x-version"));

                    })
                    .AddMvc(
                    options =>
                    {
                        // automatically applies an api version based on the name of
                        // the defining controller's namespace
                        options.Conventions.Add(new VersionByNamespaceConvention());
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
    });
}

//app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
