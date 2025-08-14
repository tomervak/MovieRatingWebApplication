using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory  _dbConnectionFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            _ = await _dbConnectionFactory.CreateConnectionAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Database is not healthy");
            return HealthCheckResult.Unhealthy("Database is not healthy", e);
        }
    }
}