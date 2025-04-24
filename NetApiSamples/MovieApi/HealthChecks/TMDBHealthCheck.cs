using CommonLibrary.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MovieApi.HealthChecks
{
    public class TMDBHealthCheck : IHealthCheck
    {
        private readonly ITMDBService _tmdbService;

        public TMDBHealthCheck(ITMDBService tmdbService)
        {
            _tmdbService = tmdbService ?? throw new ArgumentNullException(nameof(tmdbService));
        }
    
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            bool isHealthy = true;

            try
            {
                var test = await _tmdbService.GetGenresAsync();
            }
            catch (Exception ex)
            {
                isHealthy = false;
            }

            return isHealthy ? HealthCheckResult.Healthy("TMDB is active") :
                HealthCheckResult.Unhealthy("Cannont connect to TMDB");
        }
    }
}
