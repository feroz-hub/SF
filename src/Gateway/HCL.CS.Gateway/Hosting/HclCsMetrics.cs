/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Diagnostics.Metrics;

namespace HCL.CS.ProxyService.Hosting;

internal static class HclCsMetrics
{
    private static readonly Meter Meter = new("HCL.CS.Hosting.Observability", "1.0.0");

    private static readonly Counter<long> RequestCounter =
        Meter.CreateCounter<long>("hcl-cs.http.server.requests");

    private static readonly Histogram<double> RequestDurationMs =
        Meter.CreateHistogram<double>("hcl-cs.http.server.duration.ms", "ms");

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
