/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Service.Implementation.Api.Ldap;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Implementation.Api.Wrappers;
using HCL.CS.Service.Implementation.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Parsers;
using HCL.CS.Service.Implementation.Endpoint.Services;
using HCL.CS.Service.Implementation.Endpoint.Validators;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Parsers;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Results;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.Service.Extension;

public static class ServiceExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<ITokenGenerationService, TokenGenerationService>();
        services.AddTransient<IDiscoveryService, DiscoveryService>();
        services.AddTransient<IUserInfoServices, UserInfoServices>();
        services.AddTransient<IJWKSService, JWKSService>();
        services.AddTransient<IBackChannelLogoutService, BackChannelLogoutService>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapConfigurationValidator,
            LdapConfigurationValidator>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.IAuthenticationModeResolver,
            AuthenticationModeResolver>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.IAllowedEmailDomainPolicy,
            AllowedEmailDomainPolicy>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILocalAuthenticationConfigurationValidator,
            LocalAuthenticationConfigurationValidator>();
        services.AddSingleton<ILdapConnectionFactory, SystemDirectoryServicesLdapConnectionFactory>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapProtocolClient,
            SystemDirectoryServicesLdapProtocolClient>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapAccountStatusEvaluator,
            LdapAccountStatusEvaluator>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapAttributeMapper,
            LdapAttributeMapper>();
        services.AddSingleton<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapAuthenticationAuditLogger,
            LdapAuthenticationAuditLogger>();
        services.AddTransient<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapAuthenticationService,
            LdapAuthenticationService>();
        services.AddTransient<
            HCL.CS.Service.Interfaces.Interfaces.Api.ILdapAccountValidationService,
            LdapAccountValidationService>();
        services.AddTransient<
            HCL.CS.Service.Interfaces.Interfaces.Api.ISecurityAuditService,
            SecurityAuditService>();

        services.AddScoped<ISessionManagementService, SessionManagementService>();
        services.AddScoped<IInteractionService, InteractionService>();

        services.AddHttpClient<BackChannelLogoutService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        return services;
    }

    public static IServiceCollection AddWrappers(this IServiceCollection services)
    {
        services.AddTransient<IAuthenticationService, AuthenticationServiceWrapper>();
        services.AddTransient<IUserClaimsPrincipalFactory<Users>, UserClaimsPrincipalWrapper>();

        services.AddScoped<IPasswordHasher<Users>, Argon2PasswordHasherWrapper<Users>>();
        return services;
    }

    public static IServiceCollection AddDefaultEndpoints(this IServiceCollection services)
    {
        AddEndpoint<TokenEndpoint>(services, EndpointsName.Token, EndpointRoutePaths.Token.IncludedFrontSlash());
        AddEndpoint<AuthorizeEndpoint>(services, EndpointsName.Authorize,
            EndpointRoutePaths.Authorize.IncludedFrontSlash());
        AddEndpoint<AuthorizeCallBackEndpoint>(services, EndpointsName.Authorize,
            EndpointRoutePaths.AuthorizeCallback.IncludedFrontSlash());
        AddEndpoint<IntrospectionEndpoint>(services, EndpointsName.Introspection,
            EndpointRoutePaths.Introspection.IncludedFrontSlash());
        AddEndpoint<DiscoveryEndpoint>(services, EndpointsName.Discovery,
            EndpointRoutePaths.DiscoveryConfiguration.IncludedFrontSlash());
        AddEndpoint<EndSessionEndpoint>(services, EndpointsName.EndSession,
            EndpointRoutePaths.EndSession.IncludedFrontSlash());
        AddEndpoint<EndSessionCallbackEndpoint>(services, EndpointsName.EndSession,
            EndpointRoutePaths.EndSessionCallback.IncludedFrontSlash());
        AddEndpoint<TokenRevocationEndpoint>(services, EndpointsName.Revocation,
            EndpointRoutePaths.Revocation.IncludedFrontSlash());
        AddEndpoint<JwksEndpoint>(services, EndpointsName.Discovery,
            EndpointRoutePaths.JWKSWebKeys.IncludedFrontSlash());
        AddEndpoint<UserInfoEndpoint>(services, EndpointsName.UserInfo,
            EndpointRoutePaths.UserInfo.IncludedFrontSlash());
        AddEndpointValidations(services);
        return services;
    }

    private static void AddEndpoint<T>(IServiceCollection services, string name, PathString path)
        where T : class, IEndpoint
    {
        services.AddTransient<T>();
        services.AddSingleton(new SecurityEndpointModel(name, path, typeof(T)));
    }

    private static void AddEndpointValidations(IServiceCollection services)
    {
        // Token Request Validator
        services.AddTransient<ITokenRequestValidator, TokenRequestValidator>();
        services.AddTransient<ITokenParser, TokenParser>();

        // Client Validator
        services.AddTransient<IClientSecretValidator, ClientSecretValidator>();
        services.AddTransient<IClientSecretParser, ClientSecretParser>();
        services.AddTransient<ISecretValidator, SecretValidator>();

        services.AddTransient<IResourceScopeValidator, ResourceScopeValidator>();
        services.AddTransient<IIntrospectionRequestValidator, IntrospectionRequestValidator>();

        // Session Request Validator
        services.AddTransient<IEndSessionRequestValidator, EndSessionRequestValidator>();

        // Token Revocation Validator
        services.AddTransient<ITokenRevocationRequestValidator, TokenRevocationRequestValidator>();
    }
}
