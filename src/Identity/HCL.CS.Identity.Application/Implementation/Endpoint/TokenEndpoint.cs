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
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint;

public class TokenEndpoint : IEndpoint
{
    private readonly IClientSecretValidator clientSecretValidator;
    private readonly ILoggerService loggerService;
    private readonly IResourceStringHandler resourceStringHandler;
    private readonly ITokenGenerationService tokenGenerationService;
    private readonly ITokenRequestValidator tokenRequestValidator;

    public TokenEndpoint(
        ILoggerInstance instance,
        IClientSecretValidator clientSecretValidator,
        ITokenRequestValidator tokenRequestValidator,
        ITokenGenerationService tokenGenerationService,
        IResourceStringHandler resourceStringHandler)
    {
        this.clientSecretValidator = clientSecretValidator;
        this.tokenRequestValidator = tokenRequestValidator;
        this.tokenGenerationService = tokenGenerationService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.resourceStringHandler = resourceStringHandler;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        try
        {
            loggerService.WriteTo(Log.Debug, "Processing token request.");
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                loggerService.WriteTo(Log.Error, "Invalid HTTP request for token endpoint.");
                return OpenIdConstants.Errors.InvalidRequest.Error(
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidHttpRequest));
            }

            if (!context.Request.CheckHeaderContentType())
            {
                loggerService.WriteTo(Log.Error, "Invalid HTTP request for token endpoint.");
                return OpenIdConstants.Errors.InvalidRequest.Error(
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.InvalidHttpRequest));
            }

            // validate client
            var clientResult = await clientSecretValidator.ValidateClientSecretAsync(context);
            if (clientResult.IsError) return OpenIdConstants.Errors.InvalidClient.Error(clientResult.ErrorDescription);

            // validate request
            var requestCollection = (await context.Request.ReadFormAsync()).ConvertCollection();
            var validatedTokenRequestModel =
                await tokenRequestValidator.ValidateTokenRequestAsync(requestCollection, clientResult);
            if (validatedTokenRequestModel.IsError)
                return validatedTokenRequestModel.ErrorCode.Error(validatedTokenRequestModel.ErrorDescription);
            // create response
            validatedTokenRequestModel.EndpointBaseUrl = context.GetHclCsHost();
            loggerService.WriteTo(Log.Debug, "Generating token.");
            var response = await tokenGenerationService.ProcessTokenAsync(validatedTokenRequestModel);

            // Defensive guard: ensure we never send a null response into TokenResult.
            // When token generation fails (e.g. invalid/expired/invalidated refresh token),
            // return a proper OAuth error instead of throwing a NullReferenceException.
            if (response == null)
            {
                loggerService.WriteTo(Log.Error,
                    "Token generation returned null TokenResponseModel. Returning invalid_grant error.");

                return OpenIdConstants.Errors.InvalidGrant.Error(
                    resourceStringHandler.GetResourceString(EndpointErrorCodes.TokenIsNullOrInvalid));
            }

            // return result
            loggerService.WriteTo(Log.Debug, "Request successfully processed.");
            return new TokenResult(response);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, "Unhandled token endpoint exception.");
            return OpenIdConstants.Errors.ServerError.Error("Token request processing failed.");
        }
    }
}
