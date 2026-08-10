/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Specifications;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;

namespace HCL.CS.Service.Implementation.Endpoint.Validators;

internal class IntrospectionRequestValidator : IIntrospectionRequestValidator
{
    private readonly IRepository<Clients> clientRepository;
    private readonly TokenSettings configSettings;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly Dictionary<string, AsymmetricKeyInfoModel> keyStore;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly IRepository<SecurityTokens> securityTokenRepository;
    private readonly IClientsUnitOfWork unitOfWork;

    public IntrospectionRequestValidator(
        ILoggerInstance instance,
        IClientsUnitOfWork unitOfWork,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        IFrameworkResultService frameworkResultService,
        HclCsConfig tokenSettings,
        IRepository<SecurityTokens> securityTokenRepository)
    {
        this.unitOfWork = unitOfWork;
        this.clientRepository = clientRepository;
        this.mapper = mapper;
        this.keyStore = keyStore;
        this.frameworkResultService = frameworkResultService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        configSettings = tokenSettings.TokenSettings;
        this.securityTokenRepository = securityTokenRepository;
    }

    public async Task<ValidatedIntrospectionRequestModel> ValidateIntrospectionRequestAsync(
        Dictionary<string, string> requestCollection, ClientsModel client)
    {
        loggerService.WriteTo(Log.Debug, "Introspection request validation started.");
        var tokenRequest = new ValidatedIntrospectionRequestModel
        {
            RequestRawData = requestCollection,
            TokenConfigOptions = configSettings,
            Client = client,
            ClientId = client.ClientId
        };

        var requestValidator =
            new IntrospectionRequestSpecification(unitOfWork, clientRepository, mapper, keyStore,
                securityTokenRepository);
        var introspectionRequest = await requestValidator.ValidateAsync(tokenRequest);
        if (!requestValidator.IsValid)
        {
            introspectionRequest =
                frameworkResultService.Failed(introspectionRequest.ErrorCode, introspectionRequest.ErrorMessage);
            tokenRequest.ErrorCode = introspectionRequest.ErrorCode;
            tokenRequest.ErrorDescription = introspectionRequest.ErrorMessage;
        }

        // valid token
        return tokenRequest;
    }
}
