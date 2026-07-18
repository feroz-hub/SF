/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint;

internal class AuthorizeEndpoint : AuthorizationCodeBase, IEndpoint
{
    public AuthorizeEndpoint(
        ILoggerInstance instance,
        IResourceScopeValidator resourceScopeValidator,
        IAuthorizationService authorizationService,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        ISessionManagementService session,
        SignInManagerWrapper<Users> csSignInManager,
        HclCsConfig tokenSettings,
        IFrameworkResultService frameworkResultService)
        : base(
            instance,
            resourceScopeValidator,
            authorizationService,
            clientRepository,
            mapper,
            session,
            tokenSettings,
            csSignInManager,
            frameworkResultService)
    {
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        Dictionary<string, string> requestCollection;
        if (HttpMethods.IsGet(context.Request.Method))
        {
            requestCollection = context.Request.Query.ConvertCollection();
        }
        else if (HttpMethods.IsPost(context.Request.Method))
        {
            if (!context.Request.CheckHeaderContentType())
                return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);

            requestCollection = context.Request.Form.ConvertCollection();
        }
        else
        {
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        var result = await ProcessAuthorizeRequestAsync(context, requestCollection);
        LoggerService.WriteTo(Log.Debug, "End of authorize request endpoint");

        return result;
    }
}
