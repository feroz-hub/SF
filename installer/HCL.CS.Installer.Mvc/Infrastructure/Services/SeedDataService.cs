/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Enums;
using HCL.CS.Service.Implementation.Api.Wrappers;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Infrastructure.Data;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;
using HclCsInstallerMVC.Infrastructure.Seeding;

namespace HclCsInstallerMVC.Infrastructure.Services;

public sealed class SeedDataService : ISeedDataService
{
    private const string DefaultScopes =
        "openid email profile offline_access hcl-cs.apiresource phone hcl-cs.client hcl-cs.user hcl-cs.role hcl-cs.identityresource hcl-cs.adminuser hcl-cs.securitytoken";

    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(ILogger<SeedDataService> logger)
    {
        _logger = logger;
    }

    public async Task<SeedExecutionResultDto> SeedAsync(
        DatabaseConfigurationDto databaseConfiguration,
        SeedConfigurationDto seedConfiguration,
        CancellationToken cancellationToken)
    {
        try
        {
            var options = DatabaseProviderUtilities.BuildApplicationOptions(databaseConfiguration);

            await using var dbContext = new ApplicationDbContext(options);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (await dbContext.Roles.AnyAsync(cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                return new SeedExecutionResultDto
                {
                    Succeeded = false,
                    ErrorMessage = "Seed data already exists in the selected database."
                };
            }

            var apiResources = HclCsMasterDataSeed.GetApiResourceEntityMaster();
            dbContext.ApiResources.AddRange(apiResources);

            var identityResources = HclCsMasterDataSeed.CreateIdentityResourceModelMaster();
            dbContext.IdentityResources.AddRange(identityResources);

            var roles = HclCsMasterDataSeed.CreateRolesMaster();
            dbContext.Roles.AddRange(roles);

            var securityQuestions = HclCsMasterDataSeed.CreateSecurityQuestionsModelMaster();
            dbContext.SecurityQuestions.AddRange(securityQuestions);

            var adminRole = roles.First(r => r.Name == "HclCsAdmin");
            var sfAdminClaims = HclCsMasterDataSeed.CreateRoleClaims_HclCsAdmin();
            foreach (var claim in sfAdminClaims) claim.RoleId = adminRole.Id;

            dbContext.RoleClaims.AddRange(sfAdminClaims);

            var userRole = roles.First(r => r.Name == "HclCsUser");
            var sfUserClaims = HclCsMasterDataSeed.CreateRoleClaims_HclCsUser();
            foreach (var claim in sfUserClaims) claim.RoleId = userRole.Id;

            dbContext.RoleClaims.AddRange(sfUserClaims);

            var adminUser = BuildUser(seedConfiguration.AdminUser);
            dbContext.Users.Add(adminUser);

            var adminUserRole = HclCsMasterDataSeed.CreateUserRoleModelMaster();
            adminUserRole.RoleId = adminRole.Id;
            adminUserRole.UserId = adminUser.Id;
            dbContext.UserRoles.Add(adminUserRole);

            var standardUserRole = HclCsMasterDataSeed.CreateUserRoleModelMaster();
            standardUserRole.RoleId = userRole.Id;
            standardUserRole.UserId = adminUser.Id;
            dbContext.UserRoles.Add(standardUserRole);

            var (client, generatedClientSecret) = BuildClient(seedConfiguration.Client);
            dbContext.Clients.Add(client);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new SeedExecutionResultDto
            {
                Succeeded = true,
                GeneratedClientId = client.ClientId,
                GeneratedClientSecret = generatedClientSecret
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seed operation failed.");
            return new SeedExecutionResultDto
            {
                Succeeded = false,
                ErrorMessage = "Seed data generation failed. Verify supplied data and database state."
            };
        }
    }

    private static Users BuildUser(AdminUserConfigurationDto userConfiguration)
    {
        var user = HclCsMasterDataSeed.CreateUserModelMaster();
        user.UserName = userConfiguration.UserName;
        user.NormalizedUserName = userConfiguration.UserName.ToUpperInvariant();
        user.Email = userConfiguration.Email;
        user.NormalizedEmail = userConfiguration.Email.ToUpperInvariant();
        user.PhoneNumber = userConfiguration.PhoneNumber;
        user.FirstName = userConfiguration.FirstName.Trim();
        user.LastName = userConfiguration.LastName?.Trim() ?? string.Empty;
        user.IdentityProviderType = userConfiguration.IdentityProvider;
        user.AuthenticationSource = userConfiguration.IdentityProvider switch
        {
            IdentityProvider.Ldap => "LDAP",
            IdentityProvider.Google => "GOOGLE",
            _ => "LOCAL"
        };

        var hasher = new Argon2PasswordHasherWrapper<Users>();
        user.PasswordHash = hasher.HashPassword(user, userConfiguration.Password);

        return user;
    }

    private static (Clients Client, string GeneratedClientSecret) BuildClient(
        ClientConfigurationDto clientConfiguration)
    {
        var client = HclCsMasterDataSeed.CreateClientMaster();
        client.ClientName = clientConfiguration.ClientName.Trim();
        client.ClientUri = clientConfiguration.ClientUri.Trim();
        var generatedClientSecret = GenerateSecret(32);
        client.ClientId = GenerateSecret(32);
        client.ClientSecret = generatedClientSecret.Sha256();
        client.ClientIdIssuedAt = ToUnixTime(DateTime.UtcNow);
        client.ClientSecretExpiresAt = ToUnixTime(DateTime.UtcNow.AddDays(100));
        client.SupportedGrantTypes = string.Join(" ", clientConfiguration.GrantTypes);
        client.SupportedResponseTypes = string.Join(" ", clientConfiguration.ResponseTypes);
        client.RedirectUris = clientConfiguration.RedirectUris
            .Select(uri => new ClientRedirectUris
                { RedirectUri = uri, CreatedOn = DateTime.UtcNow, CreatedBy = "HclCsUser" })
            .ToList();
        client.PostLogoutRedirectUris = clientConfiguration.PostLogoutRedirectUris
            .Select(uri => new ClientPostLogoutRedirectUris
                { PostLogoutRedirectUri = uri, CreatedOn = DateTime.UtcNow, CreatedBy = "HclCsUser" })
            .ToList();

        client.FrontChannelLogoutUri = string.IsNullOrWhiteSpace(clientConfiguration.FrontChannelLogoutUri)
            ? string.Empty
            : clientConfiguration.FrontChannelLogoutUri.Trim();
        client.BackChannelLogoutUri = string.IsNullOrWhiteSpace(clientConfiguration.BackChannelLogoutUri)
            ? string.Empty
            : clientConfiguration.BackChannelLogoutUri.Trim();

        if (string.IsNullOrWhiteSpace(client.FrontChannelLogoutUri)) client.FrontChannelLogoutSessionRequired = false;

        if (string.IsNullOrWhiteSpace(client.BackChannelLogoutUri)) client.BackChannelLogoutSessionRequired = false;

        client.AllowedScopes = clientConfiguration.UseDefaultScopes
            ? DefaultScopes
            : string.Join(" ",
                clientConfiguration.AllowedScopes
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase));

        client.AllowedSigningAlgorithm = OpenIdConstants.Algorithms.RsaSha256;
        client.AccessTokenType = AccessTokenType.JWT;
        client.ApplicationType = ApplicationType.RegularWeb;

        return (client, generatedClientSecret);
    }

    private static long ToUnixTime(DateTime value)
    {
        return Convert.ToInt64(value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    private static string GenerateSecret(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
