using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Models.Endpoint;
using Zentra.Service.Implementation.Api.Wrappers;
using Zentra.Service.Implementation.Endpoint;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Parsers;
using Zentra.Service.Implementation.Endpoint.Services;
using Zentra.Service.Implementation.Endpoint.Validators;
using Zentra.Service.Interfaces.Interfaces.Endpoint;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Parsers;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Results;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;
using static Zentra.Domain.Constants.Endpoint.OpenIdConstants;

namespace Zentra.Service.Extension;

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
