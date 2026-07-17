using System.Diagnostics.Metrics;

namespace Zentra.ProxyService.Hosting;

internal static class ZentraMetrics
{
    private static readonly Meter Meter = new("Zentra.Hosting.Observability", "1.0.0");

    private static readonly Counter<long> RequestCounter =
        Meter.CreateCounter<long>("zentra.http.server.requests");

    private static readonly Histogram<double> RequestDurationMs =
        Meter.CreateHistogram<double>("zentra.http.server.duration.ms", "ms");

    internal static void RecordRequest(string method, string routeGroup, int statusCode, double durationMs)
    {
        RequestCounter.Add(
            1,
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("route", routeGroup),
            new KeyValuePair<string, object?>("status_code", statusCode));

        RequestDurationMs.Record(
            durationMs,
            new KeyValuePair<string, object?>("method", method),
            new KeyValuePair<string, object?>("route", routeGroup),
            new KeyValuePair<string, object?>("status_code", statusCode));
    }
}
