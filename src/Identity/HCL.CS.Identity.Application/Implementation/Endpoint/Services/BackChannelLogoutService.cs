/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using AutoMapper;
using Microsoft.AspNetCore.Http;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Services;

internal class BackChannelLogoutService : SecurityBase, IBackChannelLogoutService
{
    private static readonly TimeSpan BackChannelRequestTimeout = TimeSpan.FromSeconds(5);
    private readonly IRepository<Clients> clientRepository;
    private readonly IFrameworkResultService frameworkResultService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly ISessionManagementService sessionManagementService;
    private readonly ITokenGenerationService tokenGenerationService;

    public BackChannelLogoutService(
        ILoggerInstance instance,
        ITokenGenerationService tokenGenerationService,
        IFrameworkResultService frameworkResultService,
        IRepository<Clients> clientRepository,
        ISessionManagementService sessionManagementService,
        IMapper mapper,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        this.frameworkResultService = frameworkResultService;
        this.tokenGenerationService = tokenGenerationService;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
        this.clientRepository = clientRepository;
        this.sessionManagementService = sessionManagementService;
        this.mapper = mapper;
        this.httpClientFactory = httpClientFactory;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task ProcessLogoutAsync()
    {
        try
        {
            var clientIdCollection = await sessionManagementService.GetClientListAsync();
            if (clientIdCollection.ContainsAny())
            {
                var user = await sessionManagementService.GetUserPrincipalFromContextAsync();
                var sessionId = await sessionManagementService.GetSessionId();
                var subjectId = user.GetSubjectId();
                var logoutRequests = new List<Task>();

                foreach (var clientId in clientIdCollection)
                {
                    var clientCollection = await clientRepository.GetAsync(client => client.ClientId == clientId);
                    if (clientCollection.ContainsAny())
                    {
                        var client = clientCollection.FirstOrDefault();
                        var backChannelUrl = client.BackChannelLogoutUri;
                        loggerService.WriteTo(Log.Debug,
                            "Entered into Back channel logout for client :" + client.ClientName);
                        if (!string.IsNullOrWhiteSpace(backChannelUrl))
                        {
                            if (client.BackChannelLogoutSessionRequired && string.IsNullOrWhiteSpace(sessionId))
                                frameworkResultService.Throw(EndpointErrorCodes.BackChannelRequiredSessionId);

                            var clientModel = mapper.Map<Clients, ClientsModel>(client);
                            var backChannelLogoutModel = new BackChannelLogoutModel
                            {
                                ClientId = clientId,
                                LogoutUri = client.BackChannelLogoutUri,
                                SessionIdRequired = client.BackChannelLogoutSessionRequired,
                                SubjectId = subjectId,
                                SessionId = sessionId,
                                Client = clientModel
                            };

                            logoutRequests.Add(SendRequest(backChannelLogoutModel));
                        }
                    }
                }

                if (logoutRequests.Count > 0) await Task.WhenAll(logoutRequests);
            }
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    private async Task SendRequest(BackChannelLogoutModel backChannelModel)
    {
        try
        {
            var token = await tokenGenerationService.GenerateBackChannelLogoutTokenAsync(backChannelModel);
            var content = new Dictionary<string, string>();
            content.Add(OpenIdConstants.LogoutTokenEvents.LogoutToken, token);
            loggerService.WriteTo(Log.Debug, "Processing back channel logout.");
            var httpClient = httpClientFactory.CreateClient();
            var requestAborted = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(requestAborted);
            timeout.CancelAfter(BackChannelRequestTimeout);
            await httpClient.PostAsync(backChannelModel.LogoutUri, content, timeout.Token);
        }
        catch (OperationCanceledException)
        {
            loggerService.WriteTo(Log.Warning, "Back channel logout request timed out or was cancelled.");
        }
        catch (Exception ex)
        {
            loggerService.WriteTo(Log.Error, "Error : " + ex.Message);
        }
    }
}
