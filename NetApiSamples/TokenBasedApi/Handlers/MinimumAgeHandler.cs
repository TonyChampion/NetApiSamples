using Microsoft.AspNetCore.Authorization;
using TokenBasedApi.Requirements;

namespace TokenBasedApi.Handlers
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly IConfiguration _config;

        public MinimumAgeHandler(IConfiguration configuration)
        {
            _config = configuration;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var ageClaim = context.User.FindFirst(
                c => c.Type == "age" && c.Issuer == _config["Jwt:Issuer"]);

            if (ageClaim is null)
            {
                return Task.CompletedTask;
            }

            if (Convert.ToInt32(ageClaim.Value) >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
