/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;

namespace HCL.CS.Service.Implementation.Endpoint;

internal class DiscoveryEndpoint : IEndpoint
{
    private readonly IDiscoveryService discoveryService;
    private readonly ILoggerService loggerService;
    private readonly TokenSettings tokenSettings;

    public DiscoveryEndpoint(
        ILoggerInstance instance,
        IDiscoveryService discoveryService,
        HclCsConfig tokenSettings)
    {
        this.discoveryService = discoveryService;
        this.tokenSettings = tokenSettings.TokenSettings;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        loggerService.WriteTo(Log.Debug, "Processing discovery request.");

        var request = new DiscoveryRequestModel
        {
            BaseUrl = context.GetHclCsHost().IncludeEndSlash()
        };
        var result = await discoveryService.GenerateDiscoveryMetaData(request);

        loggerService.WriteTo(Log.Debug, "Discovery request successfully processed.");

        return new DiscoveryResult(result, tokenSettings.TokenConfig.CachingLifetime);
    }
}
