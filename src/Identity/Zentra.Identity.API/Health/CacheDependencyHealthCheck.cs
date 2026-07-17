using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Zentra.Hosting.Health;

public class CacheDependencyHealthCheck : IHealthCheck
{
    private static readonly TimeSpan DependencyTimeout = TimeSpan.FromSeconds(2);
    private readonly IDistributedCache distributedCache;

    public CacheDependencyHealthCheck(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var timeoutToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutToken.CancelAfter(DependencyTimeout);

        var key = $"health:{Guid.NewGuid():N}";
        var value = DateTime.UtcNow.ToString("O");
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
        };

        try
        {
            await distributedCache.SetStringAsync(key, value, entryOptions, timeoutToken.Token);
            var readValue = await distributedCache.GetStringAsync(key, timeoutToken.Token);

            if (string.Equals(value, readValue, StringComparison.Ordinal))
                return HealthCheckResult.Healthy("Cache reachable.");

            return HealthCheckResult.Unhealthy("Cache roundtrip failed.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("Cache health check timed out.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Cache health check failed.");
        }
        finally
        {
            try
            {
                await distributedCache.RemoveAsync(key, CancellationToken.None);
            }
            catch
            {
                // Best effort cleanup for the temporary key.
            }
        }
    }
}
