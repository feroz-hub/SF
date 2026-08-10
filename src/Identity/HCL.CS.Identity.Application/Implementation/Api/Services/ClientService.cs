/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Linq.Expressions;
using System.Text.Json;
using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Endpoint;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;
using HCL.CS.Service.Implementation.Api.Specifications;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public class ClientService(
    ILoggerInstance instance,
    IMapper mapper,
    IFrameworkResultService frameworkResult,
    IClientsUnitOfWork unitOfWork,
    HclCsConfig securityConfig,
    IApiResourceRepository apiResourceRepository,
    IRepository<ApiScopes> apiScopeRepository,
    IIdentityResourceRepository identityResourceRepository,
    IClientProvisioningTransactionHook provisioningTransactionHook)
    : SecurityBase, IClientServices
{
    private readonly ILoggerService loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    private readonly TokenConfig tokenConfig = securityConfig.TokenSettings.TokenConfig;
    private readonly TokenExpiration tokenExpiration = securityConfig.TokenSettings.TokenExpiration;

    public virtual async Task<ClientsModel> RegisterClientAsync(ClientsModel clientsModel)
    {
        if (clientsModel == null) frameworkResult.Throw(EndpointErrorCodes.ArgumentNullError);

        try
        {
            var clientModelValidation = new ClientModelSpecification(unitOfWork, tokenExpiration, CrudMode.Add,
                apiResourceRepository, apiScopeRepository, identityResourceRepository);
            var validationError = await clientModelValidation.ValidateAsync(clientsModel);
            if (clientModelValidation.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered into register Client :" + clientsModel.ClientName);
                var generatedClientSecret = AuthenticationConstants.KeySize32.RandomString();
                clientsModel.ClientId = AuthenticationConstants.KeySize32.RandomString();
                clientsModel.ClientIdIssuedAt = DateTime.UtcNow;
                clientsModel.ClientSecret = generatedClientSecret.Sha256();
                clientsModel.ClientSecretExpiresAt = DateTime.UtcNow.AddDays(tokenConfig.ClientSecretExpirationInDays);
                clientsModel.RequireClientSecret = true;
                clientsModel.IsFirstPartyApp = true;

                var clientEntity = mapper.Map<ClientsModel, Clients>(clientsModel);
                await unitOfWork.ClientRepository.InsertAsync(clientEntity);
                var result = await unitOfWork.SaveChangesAsync();
                if (result.Status != ResultStatus.Succeeded)
                    frameworkResult.ThrowCustomMessage(result.Errors.ToList()[0].Description);

                var registeredClient = mapper.Map<Clients, ClientsModel>(clientEntity);
                registeredClient.ClientSecret = generatedClientSecret;
                return registeredClient;
            }

            frameworkResult.Throw(validationError.ErrorCode);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }

        return null;
    }

    public virtual async Task<ClientsModel> UpdateClientAsync(ClientsModel clientsModel)
    {
        if (clientsModel == null) frameworkResult.Throw(EndpointErrorCodes.ArgumentNullError);

        try
        {
            var clientModelValidation = new ClientModelSpecification(unitOfWork, tokenExpiration, CrudMode.Update,
                apiResourceRepository, apiScopeRepository, identityResourceRepository);
            var validationError = await clientModelValidation.ValidateAsync(clientsModel);
            if (clientModelValidation.IsValid)
            {
                loggerService.WriteTo(Log.Debug, "Entered into update Client :" + clientsModel.ClientName);
                var clientsEntity = await unitOfWork.ClientRepository.GetAsync(
                    client => client.ClientId == clientsModel.ClientId,
                    new Expression<Func<Clients, object>>[]
                    {
                        client => client.RedirectUris,
                        client => client.PostLogoutRedirectUris
                    });

                if (clientsEntity.ContainsAny())
                {
                    var clientEntity = clientsEntity.FirstOrDefault();
                    var incomingSecret = clientsModel.ClientSecret;
                    var secretMatches = clientEntity.ClientSecret.CompareStrings(incomingSecret)
                                        || clientEntity.ClientSecret.CompareStrings(incomingSecret.Sha256())
                                        || clientEntity.ClientSecret.CompareStrings(incomingSecret.Sha512());
                    if (!secretMatches) frameworkResult.Throw(EndpointErrorCodes.ClientSecretInvalid);

                    var clientSecretExpiresAt = clientEntity.ClientSecretExpiresAt.ToDateTime();
                    if (DateTime.Compare(clientSecretExpiresAt, DateTime.UtcNow) < 0)
                        frameworkResult.Throw(EndpointErrorCodes.ClientSecretExpired);

                    // Update the tracked entity graph in place. Server-managed fields (Id, ClientId, ClientSecret,
                    // ClientIdIssuedAt, ClientSecretExpiresAt, CreatedOn/CreatedBy and the RowVersion concurrency
                    // token) are intentionally NOT overwritten from the client payload. The child URI collections
                    // are reconciled against the loaded, tracked rows so EF Core emits correct UPDATE / INSERT /
                    // soft-DELETE statements without detaching rows, duplicating unique (ClientId, Uri) keys, or
                    // writing payload timestamps (DateTimeKind.Unspecified) into the audit columns.
                    var actor = ResolveModifier(clientsModel, clientEntity);
                    ApplyEditableClientFields(clientEntity, clientsModel);
                    // Always record the update on the client row so an unchanged-but-resubmitted client (for
                    // example a scope-only edit or a repeat submit) still persists as a modification rather than
                    // returning "No changes written". CreatedOn/CreatedBy remain untouched.
                    clientEntity.ModifiedBy = actor;
                    clientEntity.ModifiedOn = DateTime.UtcNow;
                    await ReconcileRedirectUrisAsync(clientEntity, clientsModel.RedirectUris, actor);
                    await ReconcilePostLogoutRedirectUrisAsync(clientEntity, clientsModel.PostLogoutRedirectUris, actor);

                    var result = await unitOfWork.SaveChangesAsync();
                    if (result.Status != ResultStatus.Succeeded)
                        frameworkResult.ThrowCustomMessage(result.Errors.ToList()[0].Description);

                    return mapper.Map<Clients, ClientsModel>(clientEntity);
                }

                frameworkResult.Throw(EndpointErrorCodes.ClientDoesNotExist);
            }
            else
            {
                frameworkResult.Throw(validationError.ErrorCode);
            }
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }

        return null;
    }

    public virtual async Task<FrameworkResult> DeleteClientAsync(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return frameworkResult.Failed<FrameworkResult>(EndpointErrorCodes.ClientIdIsRequired);

        try
        {
            var clientsEntity = await GetClientDetailsAsync(clientId);
            if (clientsEntity != null)
            {
                await using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    loggerService.WriteTo(Log.Debug, "Entered into remove Client :" + clientsEntity.ClientName);
                    await unitOfWork.ClientRepository.DeleteAsync(clientsEntity);
                    var result = await unitOfWork.SaveChangesAsync();
                    if (result.Status == ResultStatus.Succeeded)
                        result = await DeleteClientTokens(clientsEntity.ClientId);

                    if (result.Status == ResultStatus.Succeeded)
                        await transaction.CommitAsync();
                    else
                        await transaction.RollbackAsync();

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return frameworkResult.Failed<FrameworkResult>(EndpointErrorCodes.ClientDoesNotExist);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public virtual async Task<ClientsModel> GetClientAsync(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId)) frameworkResult.Throw(EndpointErrorCodes.InactiveClient);

        ClientsModel clientModel;
        try
        {
            var clientsEntity = await GetClientDetailsAsync(clientId);
            if (clientsEntity != null)
            {
                clientModel = mapper.Map<Clients, ClientsModel>(clientsEntity);
                loggerService.WriteTo(Log.Debug, "Entered into Get client by id : " + clientModel.ClientName);
                if (clientModel.RequireClientSecret
                    && DateTime.Compare(clientModel.ClientSecretExpiresAt, DateTime.UtcNow) < 0)
                    frameworkResult.Throw(EndpointErrorCodes.ClientSecretExpired);

                return clientModel;
            }

            return frameworkResult.EmptyResult<ClientsModel>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public virtual async Task<Dictionary<string, string>> GetAllClientAsync()
    {
        try
        {
            var clientList = new Dictionary<string, string>();
            var clientsEntityList = await unitOfWork.ClientRepository.GetAllAsync();
            if (clientsEntityList.ContainsAny())
            {
                loggerService.WriteTo(Log.Debug, "Entered into get all clients - Count : " + clientsEntityList.Count);
                foreach (var clientEntity in clientsEntityList)
                    if (!clientList.ContainsKey(clientEntity.ClientId))
                        clientList.Add(clientEntity.ClientId, clientEntity.ClientName);

                return clientList;
            }

            return frameworkResult.EmptyResult<Dictionary<string, string>>(ApiErrorCodes.NoRecordsFound);
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    public virtual async Task<ClientsModel> GenerateClientSecret(string clientId)
    {
        // Change to specific required error code for clientId and clientSecret.
        if (string.IsNullOrWhiteSpace(clientId)) frameworkResult.Throw(EndpointErrorCodes.ClientIdIsRequired);

        var clientModel = new ClientsModel();
        try
        {
            var clientEntityList = await unitOfWork.ClientRepository.GetAsync(client =>
                client.ClientId == clientId);
            if (clientEntityList.ContainsAny())
            {
                var clientEntity = clientEntityList.FirstOrDefault();
                loggerService.WriteTo(Log.Debug,
                    "Entered into Generate Client secret for Client :" + clientEntity.ClientName);

                var generatedClientSecret = AuthenticationConstants.KeySize32.RandomString();
                clientEntity.ClientSecret = generatedClientSecret.Sha256();
                clientEntity.ClientSecretExpiresAt =
                    DateTime.UtcNow.AddDays(tokenConfig.ClientSecretExpirationInDays).ToUnixTime();

                await unitOfWork.ClientRepository.UpdateAsync(clientEntity);
                await unitOfWork.SaveChangesAsync();

                clientModel = mapper.Map<Clients, ClientsModel>(clientEntity);
                clientModel.ClientSecret = generatedClientSecret;
            }
            else
            {
                frameworkResult.Throw(EndpointErrorCodes.ClientDoesNotExist);
            }
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }

        return clientModel;
    }

    private async Task<Clients> GetClientDetailsAsync(string clientId)
    {
        Clients clientEntity = null;
        try
        {
            var clientsEntity = await unitOfWork.ClientRepository.GetAsync(client => client.ClientId == clientId,
                new System.Linq.Expressions.Expression<Func<Clients, object>>[] { x => x.RedirectUris, x => x.PostLogoutRedirectUris });
            if (clientsEntity.ContainsAny())
            {
                clientEntity = clientsEntity.ToList()[0];
                loggerService.WriteTo(Log.Debug,
                    "Entered into get client :" + clientEntity.ClientName); // To Be Discussed.
            }

            return clientEntity;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    private static string ResolveModifier(ClientsModel clientsModel, Clients clientEntity)
    {
        if (!string.IsNullOrWhiteSpace(clientsModel.ModifiedBy)) return clientsModel.ModifiedBy;
        if (!string.IsNullOrWhiteSpace(clientsModel.CreatedBy)) return clientsModel.CreatedBy;
        return string.IsNullOrWhiteSpace(clientEntity.CreatedBy) ? "hcl-cs-admin" : clientEntity.CreatedBy;
    }

    private static void ApplyEditableClientFields(Clients clientEntity, ClientsModel clientsModel)
    {
        // Only client-editable fields are copied. Identity, secret, issuance/expiry, audit and concurrency
        // fields are deliberately preserved from the loaded entity.
        clientEntity.ClientName = clientsModel.ClientName;
        clientEntity.ClientUri = clientsModel.ClientUri;
        clientEntity.LogoUri = clientsModel.LogoUri;
        clientEntity.TermsOfServiceUri = clientsModel.TermsOfServiceUri;
        clientEntity.PolicyUri = clientsModel.PolicyUri;
        clientEntity.RefreshTokenExpiration = clientsModel.RefreshTokenExpiration;
        clientEntity.AccessTokenExpiration = clientsModel.AccessTokenExpiration;
        clientEntity.IdentityTokenExpiration = clientsModel.IdentityTokenExpiration;
        clientEntity.LogoutTokenExpiration = clientsModel.LogoutTokenExpiration;
        clientEntity.AuthorizationCodeExpiration = clientsModel.AuthorizationCodeExpiration;
        clientEntity.AccessTokenType = clientsModel.AccessTokenType;
        clientEntity.RequirePkce = clientsModel.RequirePkce;
        clientEntity.IsPkceTextPlain = clientsModel.IsPkceTextPlain;
        clientEntity.RequireClientSecret = true;
        clientEntity.IsFirstPartyApp = true;
        clientEntity.AllowOfflineAccess = clientsModel.AllowOfflineAccess;
        clientEntity.AllowAccessTokensViaBrowser = clientsModel.AllowAccessTokensViaBrowser;
        clientEntity.ApplicationType = clientsModel.ApplicationType;
        clientEntity.AllowedSigningAlgorithm = clientsModel.AllowedSigningAlgorithm;
        clientEntity.FrontChannelLogoutSessionRequired = clientsModel.FrontChannelLogoutSessionRequired;
        clientEntity.FrontChannelLogoutUri = clientsModel.FrontChannelLogoutUri;
        clientEntity.BackChannelLogoutSessionRequired = clientsModel.BackChannelLogoutSessionRequired;
        clientEntity.BackChannelLogoutUri = clientsModel.BackChannelLogoutUri;
        clientEntity.PreferredAudience = clientsModel.PreferredAudience;
        clientEntity.AllowedScopes = string.Join(" ", clientsModel.AllowedScopes ?? new List<string>());
        clientEntity.SupportedGrantTypes = string.Join(" ", clientsModel.SupportedGrantTypes ?? new List<string>());
        clientEntity.SupportedResponseTypes = string.Join(" ", clientsModel.SupportedResponseTypes ?? new List<string>());
    }

    private async Task ReconcileRedirectUrisAsync(Clients clientEntity, List<ClientRedirectUrisModel> incoming, string actor)
    {
        clientEntity.RedirectUris ??= new List<ClientRedirectUris>();
        var incomingUris = (incoming ?? new List<ClientRedirectUrisModel>())
            .Select(uri => uri.RedirectUri)
            .Where(uri => !string.IsNullOrWhiteSpace(uri))
            .Distinct()
            .ToList();

        foreach (var removed in clientEntity.RedirectUris
                     .Where(existing => incomingUris.All(uri => uri != existing.RedirectUri))
                     .ToList())
            await unitOfWork.RedirectUrisRepository.DeleteAsync(removed);

        foreach (var uri in incomingUris.Where(uri => clientEntity.RedirectUris.All(existing => existing.RedirectUri != uri)))
            // Id is left as the default (empty) value: the key is store-generated, so EF Core treats the row as
            // Added and INSERTs it (a non-empty key would be read as an existing row and issue a failing UPDATE).
            clientEntity.RedirectUris.Add(new ClientRedirectUris
            {
                ClientId = clientEntity.Id,
                RedirectUri = uri,
                CreatedBy = actor,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            });
    }

    private async Task ReconcilePostLogoutRedirectUrisAsync(Clients clientEntity,
        List<ClientPostLogoutRedirectUrisModel> incoming, string actor)
    {
        clientEntity.PostLogoutRedirectUris ??= new List<ClientPostLogoutRedirectUris>();
        var incomingUris = (incoming ?? new List<ClientPostLogoutRedirectUrisModel>())
            .Select(uri => uri.PostLogoutRedirectUri)
            .Where(uri => !string.IsNullOrWhiteSpace(uri))
            .Distinct()
            .ToList();

        foreach (var removed in clientEntity.PostLogoutRedirectUris
                     .Where(existing => incomingUris.All(uri => uri != existing.PostLogoutRedirectUri))
                     .ToList())
            await unitOfWork.PostLogoutRedirectUrisRepository.DeleteAsync(removed);

        foreach (var uri in incomingUris.Where(uri =>
                     clientEntity.PostLogoutRedirectUris.All(existing => existing.PostLogoutRedirectUri != uri)))
            // Id left as default (empty): store-generated key => EF Core treats the row as Added and INSERTs it.
            clientEntity.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUris
            {
                ClientId = clientEntity.Id,
                PostLogoutRedirectUri = uri,
                CreatedBy = actor,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            });
    }

    private async Task<FrameworkResult> DeleteClientTokens(string clientId)
    {
        var securityTokenList = await unitOfWork.SecurityTokensRepository.GetAsync(x => x.ClientId == clientId);
        if (securityTokenList.ContainsAny())
        {
            await unitOfWork.SecurityTokensRepository.DeleteAsync(securityTokenList);
            return await unitOfWork.SaveChangesWithHardDeleteAsync();
        }

        return frameworkResult.Succeeded();
    }

    public virtual async Task<ClientsModel> ProvisionClientAsync(ClientsModel clientsModel)
    {
        if (clientsModel == null) frameworkResult.Throw(EndpointErrorCodes.ArgumentNullError);

        NormalizeProvisioningRequest(clientsModel);
        await ValidateProvisioningRequestAsync(clientsModel);

        var existingList = await unitOfWork.ClientRepository.GetAsync(
            client => client.ClientId == clientsModel.ClientId,
            new System.Linq.Expressions.Expression<Func<Clients, object>>[]
            {
                client => client.RedirectUris,
                client => client.PostLogoutRedirectUris
            });
        var existingEntity = existingList?.FirstOrDefault();
        if (existingEntity is not null)
        {
            EnsureIdempotentDefinition(existingEntity, clientsModel);
            var existingModel = mapper.Map<Clients, ClientsModel>(existingEntity);
            existingModel.ClientSecret = null;
            loggerService.WriteTo(
                Log.Information,
                $"Client provisioning request was idempotent for client: {clientsModel.ClientId}");
            return existingModel;
        }

        var rawSecret = clientsModel.RequireClientSecret
            ? string.IsNullOrWhiteSpace(clientsModel.ClientSecret)
                ? AuthenticationConstants.KeySize32.RandomString()
                : clientsModel.ClientSecret
            : null;
        clientsModel.ClientSecret = rawSecret?.Sha256();
        clientsModel.ClientIdIssuedAt = DateTime.UtcNow;
        clientsModel.ClientSecretExpiresAt = clientsModel.RequireClientSecret
            ? DateTime.UtcNow.AddDays(tokenConfig.ClientSecretExpirationInDays)
            : DateTime.UnixEpoch;

        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var clientEntity = mapper.Map<ClientsModel, Clients>(clientsModel);
            await unitOfWork.ClientRepository.InsertAsync(clientEntity);
            await unitOfWork.AuditTrailRepository.InsertAsync(new AuditTrail
            {
                Id = Guid.NewGuid(),
                ActionType = AuditType.Create,
                TableName = "HclCs_Clients",
                ActionName = "ProvisionClient",
                NewValue = JsonSerializer.Serialize(new
                {
                    clientsModel.ClientId,
                    clientsModel.ClientName,
                    ClientType = clientsModel.ApplicationType.ToString(),
                    clientsModel.RequirePkce,
                    clientsModel.RequireClientSecret,
                    clientsModel.PreferredAudience
                }),
                AffectedColumn = "ClientId",
                CreatedBy = clientsModel.CreatedBy,
                CreatedOn = DateTime.UtcNow
            });

            var saveResult = await unitOfWork.SaveChangesAsync();
            if (saveResult.Status != ResultStatus.Succeeded)
            {
                frameworkResult.ThrowCustomMessage(
                    saveResult.Errors.FirstOrDefault()?.Description
                    ?? "Failed to persist provisioned client.");
            }

            await provisioningTransactionHook.BeforeCommitAsync(clientsModel.ClientId);
            await transaction.CommitAsync();

            var registeredModel = mapper.Map<Clients, ClientsModel>(clientEntity);
            registeredModel.ClientSecret = rawSecret;
            loggerService.WriteTo(Log.Information, $"Provisioned client: {clientsModel.ClientId}");
            return registeredModel;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            loggerService.WriteToWithCaller(
                Log.Error,
                ex,
                $"Client provisioning transaction rolled back for client: {clientsModel.ClientId}");
            throw;
        }
    }

    private async Task ValidateProvisioningRequestAsync(ClientsModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ClientId))
            throw new InvalidOperationException("CLIENT_ID_REQUIRED: clientId is required.");
        if (string.IsNullOrWhiteSpace(model.ClientName))
            throw new InvalidOperationException("CLIENT_NAME_REQUIRED: clientName is required.");
        if (!Enum.IsDefined(model.ApplicationType))
            throw new InvalidOperationException("UNSUPPORTED_CLIENT_TYPE: applicationType is not supported.");

        var allowedGrantTypes = new HashSet<string>(
            new[]
            {
                OpenIdConstants.GrantTypes.AuthorizationCode,
                OpenIdConstants.GrantTypes.RefreshToken,
                OpenIdConstants.GrantTypes.ClientCredentials
            },
            StringComparer.Ordinal);
        if (model.SupportedGrantTypes.Count == 0
            || model.SupportedGrantTypes.Any(grant => !allowedGrantTypes.Contains(grant)))
        {
            throw new InvalidOperationException("UNSUPPORTED_GRANT_TYPE: requested grant type is not supported.");
        }

        var isPublic = model.ApplicationType is ApplicationType.SinglePageApp or ApplicationType.Native;
        var isService = model.ApplicationType == ApplicationType.Service;
        if (isPublic)
        {
            if (model.RequireClientSecret || !string.IsNullOrWhiteSpace(model.ClientSecret))
                throw new InvalidOperationException("PUBLIC_CLIENT_SECRET_FORBIDDEN: public clients cannot have a secret.");
            if (!model.RequirePkce)
                throw new InvalidOperationException("PUBLIC_CLIENT_PKCE_REQUIRED: public clients must require PKCE.");
            if (!model.SupportedGrantTypes.Contains(OpenIdConstants.GrantTypes.AuthorizationCode, StringComparer.Ordinal)
                || model.SupportedGrantTypes.Contains(OpenIdConstants.GrantTypes.ClientCredentials, StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    "INVALID_PUBLIC_CLIENT_GRANTS: public clients must use authorization_code and cannot use client_credentials.");
            }
        }
        else if (isService)
        {
            if (!model.RequireClientSecret)
                throw new InvalidOperationException("CONFIDENTIAL_CLIENT_SECRET_REQUIRED: service clients require credentials.");
            if (model.SupportedGrantTypes.Count != 1
                || !model.SupportedGrantTypes.Contains(
                    OpenIdConstants.GrantTypes.ClientCredentials,
                    StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    "INVALID_SERVICE_CLIENT_GRANTS: service clients must use only client_credentials.");
            }
            if (model.RedirectUris.Count > 0)
                throw new InvalidOperationException("SERVICE_CLIENT_REDIRECT_FORBIDDEN: service clients cannot use redirect URIs.");
        }
        else
        {
            if (!model.RequireClientSecret)
                throw new InvalidOperationException("CONFIDENTIAL_CLIENT_SECRET_REQUIRED: web clients require credentials.");
            if (!model.SupportedGrantTypes.Contains(OpenIdConstants.GrantTypes.AuthorizationCode, StringComparer.Ordinal)
                || model.SupportedGrantTypes.Contains(OpenIdConstants.GrantTypes.ClientCredentials, StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    "INVALID_WEB_CLIENT_GRANTS: web clients must use authorization_code and cannot use client_credentials.");
            }
        }

        var usesAuthorizationCode = model.SupportedGrantTypes.Contains(
            OpenIdConstants.GrantTypes.AuthorizationCode,
            StringComparer.Ordinal);
        if (usesAuthorizationCode)
        {
            if (model.RedirectUris.Count == 0)
                throw new InvalidOperationException("REDIRECT_URI_REQUIRED: authorization-code clients require a redirect URI.");
            if (!model.SupportedResponseTypes.Contains(OpenIdConstants.ResponseTypes.Code, StringComparer.Ordinal))
                throw new InvalidOperationException("RESPONSE_TYPE_CODE_REQUIRED: authorization-code clients require response type code.");
        }

        foreach (var redirectUri in model.RedirectUris.Select(uri => uri.RedirectUri)
                     .Concat(model.PostLogoutRedirectUris.Select(uri => uri.PostLogoutRedirectUri)))
        {
            if (string.IsNullOrWhiteSpace(redirectUri)
                || redirectUri.Contains('*', StringComparison.Ordinal)
                || !Uri.TryCreate(redirectUri, UriKind.Absolute, out var parsed)
                || !string.Equals(parsed.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "INVALID_REDIRECT_URI: redirect URIs must be exact absolute HTTPS URIs without wildcards.");
            }
        }

        var identityResources = (await identityResourceRepository.GetAllAsync())
            .Select(resource => resource.Name)
            .ToHashSet(StringComparer.Ordinal);
        var apiResources = (await apiResourceRepository.GetAllAsync())
            .Select(resource => resource.Name)
            .ToHashSet(StringComparer.Ordinal);
        var apiScopes = (await apiScopeRepository.GetAllAsync())
            .Select(scope => scope.Name)
            .ToHashSet(StringComparer.Ordinal);
        var knownScopes = identityResources
            .Concat(apiResources)
            .Concat(apiScopes)
            .Append(AuthenticationConstants.IdentityScopes.OfflineAccess)
            .ToHashSet(StringComparer.Ordinal);

        var unknownScopes = model.AllowedScopes.Where(scope => !knownScopes.Contains(scope)).ToArray();
        if (model.AllowedScopes.Count == 0 || unknownScopes.Length > 0)
            throw new InvalidOperationException("UNKNOWN_SCOPE: one or more requested scopes are not registered.");
        if (isService && model.AllowedScopes.Any(scope =>
                identityResources.Contains(scope)
                || string.Equals(
                    scope,
                    AuthenticationConstants.IdentityScopes.OfflineAccess,
                    StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(
                "INVALID_SERVICE_SCOPE: service clients may request only registered API scopes.");
        }

        var knownAudiences = apiResources
            .Append(tokenConfig.ApiIdentifier)
            .Where(audience => !string.IsNullOrWhiteSpace(audience))
            .ToHashSet(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(model.PreferredAudience)
            || !knownAudiences.Contains(model.PreferredAudience))
        {
            throw new InvalidOperationException("UNKNOWN_AUDIENCE: preferredAudience is not registered.");
        }
    }

    private static void NormalizeProvisioningRequest(ClientsModel model)
    {
        model.ClientId = model.ClientId?.Trim();
        model.ClientName = model.ClientName?.Trim();
        model.PreferredAudience = model.PreferredAudience?.Trim();
        model.CreatedBy = string.IsNullOrWhiteSpace(model.CreatedBy)
            ? "HCL.CS Management API"
            : model.CreatedBy.Trim();
        model.AllowedScopes = NormalizeValues(model.AllowedScopes);
        model.SupportedGrantTypes = NormalizeValues(model.SupportedGrantTypes);
        model.SupportedResponseTypes = NormalizeValues(model.SupportedResponseTypes);
        model.RedirectUris ??= new List<ClientRedirectUrisModel>();
        model.PostLogoutRedirectUris ??= new List<ClientPostLogoutRedirectUrisModel>();

        EnsureNoDuplicates(
            model.RedirectUris.Select(uri => uri.RedirectUri),
            "DUPLICATE_REDIRECT_URI");
        EnsureNoDuplicates(
            model.PostLogoutRedirectUris.Select(uri => uri.PostLogoutRedirectUri),
            "DUPLICATE_POST_LOGOUT_REDIRECT_URI");
        model.RedirectUris = model.RedirectUris
            .OrderBy(uri => uri.RedirectUri, StringComparer.Ordinal)
            .ToList();
        model.PostLogoutRedirectUris = model.PostLogoutRedirectUris
            .OrderBy(uri => uri.PostLogoutRedirectUri, StringComparer.Ordinal)
            .ToList();
    }

    private static List<string> NormalizeValues(IEnumerable<string> values)
    {
        var normalized = (values ?? Array.Empty<string>())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList();
        EnsureNoDuplicates(normalized, "DUPLICATE_VALUE");
        return normalized.OrderBy(value => value, StringComparer.Ordinal).ToList();
    }

    private static void EnsureNoDuplicates(IEnumerable<string> values, string errorCode)
    {
        var materialized = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList();
        if (materialized.Count != materialized.Distinct(StringComparer.Ordinal).Count())
            throw new InvalidOperationException($"{errorCode}: duplicate values are not allowed.");
    }

    private static void EnsureIdempotentDefinition(Clients existing, ClientsModel requested)
    {
        var existingScopes = SplitValues(existing.AllowedScopes);
        var existingGrants = SplitValues(existing.SupportedGrantTypes);
        var existingResponses = SplitValues(existing.SupportedResponseTypes);
        var existingRedirects = existing.RedirectUris?
            .Select(uri => uri.RedirectUri)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();
        var requestedRedirects = requested.RedirectUris
            .Select(uri => uri.RedirectUri)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        var definitionMatches =
            string.Equals(existing.ClientName, requested.ClientName, StringComparison.Ordinal)
            && existing.ApplicationType == requested.ApplicationType
            && existing.RequirePkce == requested.RequirePkce
            && existing.RequireClientSecret == requested.RequireClientSecret
            && existing.AllowOfflineAccess == requested.AllowOfflineAccess
            && string.Equals(existing.PreferredAudience, requested.PreferredAudience, StringComparison.Ordinal)
            && existingScopes.SequenceEqual(requested.AllowedScopes, StringComparer.Ordinal)
            && existingGrants.SequenceEqual(requested.SupportedGrantTypes, StringComparer.Ordinal)
            && existingResponses.SequenceEqual(requested.SupportedResponseTypes, StringComparer.Ordinal)
            && existingRedirects.SequenceEqual(requestedRedirects, StringComparer.Ordinal);

        if (definitionMatches
            && !string.IsNullOrWhiteSpace(requested.ClientSecret)
            && !string.Equals(
                existing.ClientSecret,
                requested.ClientSecret.Sha256(),
                StringComparison.Ordinal))
        {
            definitionMatches = false;
        }

        if (!definitionMatches)
            throw new InvalidOperationException(
                "CLIENT_DEFINITION_CONFLICT: the client ID already exists with a different definition.");
    }

    private static string[] SplitValues(string values)
    {
        return (values ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
    }
}
