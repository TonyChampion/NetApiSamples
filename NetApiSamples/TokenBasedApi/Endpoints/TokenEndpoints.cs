using CommonLibrary.Models.TMDB;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TokenBasedApi.Endpoints
{
    public static class TokenEndpoints
    {
        public static string TokenTag = "token";

        public static void MapTokenEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("token/get", (IConfiguration config, ITokenService tokenService, int age) =>
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                string id = Guid.NewGuid().ToString();

                var claims = new List<Claim>()
            {
                new Claim("age", age.ToString()),
                new Claim("id", id)
            };

                var secToken = new JwtSecurityToken(config["Jwt:Issuer"],
                  config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(secToken);

                tokenService.AddToken(id, token);

                return token;
            })
            .WithTags(TokenTag)
            .Produces<string>(StatusCodes.Status200OK);
        }
    }
}
