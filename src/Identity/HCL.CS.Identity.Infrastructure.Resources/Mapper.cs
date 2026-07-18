/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace HCL.CS.Infrastructure.Resources;

public class Mapper
{
    public MapperConfiguration InitializeMapper()
    {
        var licenseKey = Environment.GetEnvironmentVariable("HCL_CS_AUTOMAPPER_LICENSE_KEY");
        var configuration = new MapperConfiguration(config =>
        {
            if (!string.IsNullOrWhiteSpace(licenseKey))
            {
                config.LicenseKey = licenseKey;
            }

            // .NET 8 introduces LINQ generic math helpers that can trigger
            // AutoMapper method-discovery reflection issues on startup.
            config.Internal().MethodMappingEnabled = false;
            config.ShouldMapMethod = _ => false;

            config.AddCollectionMappers();
            AuditMapper(config);
            NotificationMapper(config);
            AuthenticationMapper(config);
            UserManagementMapper(config);
            AuthorizationMapper(config);
        }, NullLoggerFactory.Instance);
        return configuration;
    }

    private static void UserManagementMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<Users, UserDisplayModel>();
        config.CreateMap<UserModel, Users>().EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<Users, UserModel>();
        config.CreateMap<SecurityQuestionModel, SecurityQuestions>()
            .EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<SecurityQuestions, SecurityQuestionModel>();
        config.CreateMap<UserSecurityQuestionModel, UserSecurityQuestions>()
            .EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<UserSecurityQuestions, UserSecurityQuestionModel>();
        config.CreateMap<UserClaimModel, UserClaims>().EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<UserClaims, UserClaimModel>();
    }

    private static void AuthenticationMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<ClientRedirectUrisModel, ClientRedirectUris>().ReverseMap();
        config.CreateMap<ClientPostLogoutRedirectUrisModel, ClientPostLogoutRedirectUris>().ReverseMap();
        config.CreateMap<ClientsModel, Clients>()
            .ForMember(dest => dest.ClientIdIssuedAt, opt => opt.MapFrom(src => src.ClientIdIssuedAt.ToUnixTime()))
            .ForMember(desc => desc.AllowedScopes,
                opt => opt.MapFrom(src => string.Join(" ", src.AllowedScopes.ToArray())))
            .ForMember(desc => desc.SupportedGrantTypes,
                opt => opt.MapFrom(src => string.Join(" ", src.SupportedGrantTypes.ToArray())))
            .ForMember(desc => desc.SupportedResponseTypes,
                opt => opt.MapFrom(src => string.Join(" ", src.SupportedResponseTypes.ToArray())))
            .ForMember(
                dest => dest.ClientSecretExpiresAt,
                opt => opt.MapFrom(src => src.ClientSecretExpiresAt.ToUnixTime()));

        config.CreateMap<Clients, ClientsModel>()
            .ForMember(dest => dest.ClientIdIssuedAt, opt => opt.MapFrom(src => src.ClientIdIssuedAt.ToDateTime()))
            .ForMember(desc => desc.AllowedScopes,
                opt => opt.MapFrom(src => src.AllowedScopes.Split(' ', StringSplitOptions.None).ToList()))
            .ForMember(desc => desc.SupportedGrantTypes,
                opt => opt.MapFrom(src => src.SupportedGrantTypes.Split(' ', StringSplitOptions.None).ToList()))
            .ForMember(desc => desc.SupportedResponseTypes,
                opt => opt.MapFrom(src => src.SupportedResponseTypes.Split(' ', StringSplitOptions.None).ToList()))
            .ForMember(
                dest => dest.ClientSecretExpiresAt,
                opt => opt.MapFrom(src => src.ClientSecretExpiresAt.ToDateTime()));

        config.CreateMap<IdentityResourcesModel, IdentityResources>()
            .EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<IdentityClaimsModel, IdentityClaims>().EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<IdentityResources, IdentityResourcesModel>();
        config.CreateMap<IdentityClaims, IdentityClaimsModel>();

        config.CreateMap<ApiResourcesModel, ApiResources>()
            .ForMember(dest => dest.RowVersion, opt => opt.UseDestinationValue())
            .EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<ApiResourceClaimsModel, ApiResourceClaims>()
            .ForMember(dest => dest.RowVersion, opt => opt.UseDestinationValue())
            .EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<ApiScopesModel, ApiScopes>()
            .ForMember(dest => dest.RowVersion, opt => opt.UseDestinationValue())
            .EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<ApiScopeClaimsModel, ApiScopeClaims>()
            .ForMember(dest => dest.RowVersion, opt => opt.UseDestinationValue())
            .EqualityComparison((src, dest) => src.Id == dest.Id);

        config.CreateMap<ApiResources, ApiResourcesModel>();
        config.CreateMap<ApiResourceClaims, ApiResourceClaimsModel>();
        config.CreateMap<ApiScopes, ApiScopesModel>();
        config.CreateMap<ApiScopeClaims, ApiScopeClaimsModel>();
        config.CreateMap<SecurityTokens, SecurityTokensModel>().ReverseMap();
    }

    private static void AuthorizationMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<RoleModel, Roles>().EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<RoleClaimModel, RoleClaims>().EqualityComparison((src, dest) => src.Id == dest.Id);
        config.CreateMap<Roles, RoleModel>();
        config.CreateMap<RoleClaims, RoleClaimModel>();
        config.CreateMap<UserRoleModel, UserRoles>().ReverseMap();
    }

    private static void AuditMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<AuditTrailModel, AuditTrail>()
            .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ActionType.ToString())).ReverseMap();
    }

    private static void NotificationMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<NotificationInfoModel, EmailMessageModel>();
        config.CreateMap<NotificationInfoModel, SMSMessage>()
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.ToAddress));
        config.CreateMap<EmailMessageModel, Notification>()
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.ToAddress))
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.FromAddress))
            .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.Activity));

        config.CreateMap<SMSMessage, Notification>()
            .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.Activity));
    }
}
