using CommonLibrary;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;
using TokenBasedApi.Handlers;
using TokenBasedApi.Requirements;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AtLeast21", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(21)));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        
     };

     options.Events = new JwtBearerEvents
     {
         OnTokenValidated = async ctx =>
         {
             var idClaim = ctx.Principal.Claims.FirstOrDefault(c => c.Type == "id");
             if(idClaim != null)
             {
                var tokenService = ctx.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                if(tokenService.IsValid(idClaim.Value))
                 {
                     ctx.Success();
                     return;
                 }
             }

             ctx.Fail("token is invalid");
         }
     };
 });

// Add services to the container.
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddHttpClient<ITMDBService, TMDBService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(TMDBService.BaseUrl);
    var test = configuration[GlobalConstants.TMDBApiKey];
    client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", configuration[GlobalConstants.TMDBApiKey]);
});

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
