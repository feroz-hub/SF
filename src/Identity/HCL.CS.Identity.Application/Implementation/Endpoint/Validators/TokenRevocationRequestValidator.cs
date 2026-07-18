/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Specifications;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint.Validators;

internal class TokenRevocationRequestValidator : ITokenRevocationRequestValidator
{
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly ILoggerService loggerService;
    private readonly IRepository<SecurityTokens> securityTokenRepository;

    public TokenRevocationRequestValidator(
        ILoggerInstance instance,
        IFrameworkResultService frameworkResultService,
        IRepository<SecurityTokens> securityTokenRepository,
        HclCsConfig tokenSettings)
    {
        configSettings = tokenSettings.TokenSettings;
        this.frameworkResultService = frameworkResultService;
        this.securityTokenRepository = securityTokenRepository;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<ValidatedRevocationRequestModel> ValidateRevocationRequestAsync(
        Dictionary<string, string> requestCollection,
        ClientsModel client)
    {
        loggerService.WriteTo(Log.Debug, "Entered into revocation request validation.");

        var validatedRequestModel = new ValidatedRevocationRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings,
            Client = client,
            ClientId = client?.ClientId
        };

        var tokenRevocationRequestValidation = new TokenRevocationRequestSpecification(securityTokenRepository);
        var validationError = await tokenRevocationRequestValidation.ValidateAsync(validatedRequestModel);
        if (!tokenRevocationRequestValidation.IsValid)
        {
            validationError = frameworkResultService.Failed(validationError.ErrorCode, validationError.ErrorMessage);
            validatedRequestModel.ErrorCode = validationError.ErrorCode;
            validatedRequestModel.ErrorDescription = validationError.ErrorMessage;
            return validatedRequestModel;
        }

        validatedRequestModel.IsError = false;
        return validatedRequestModel;
    }
}
