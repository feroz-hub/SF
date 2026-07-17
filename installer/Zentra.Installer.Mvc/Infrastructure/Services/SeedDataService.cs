using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Enums;
using Zentra.Service.Implementation.Api.Wrappers;
using Zentra.Service.Implementation.Endpoint.Extensions;
using ZentraInstallerMVC.Application.Abstractions;
using ZentraInstallerMVC.Application.DTOs;
using ZentraInstallerMVC.Infrastructure.Persistence.Data;
using ZentraInstallerMVC.Infrastructure.Seeding;

namespace ZentraInstallerMVC.Infrastructure.Services;

public sealed class SeedDataService : ISeedDataService
{
    private const string DefaultScopes =
        "openid email profile offline_access zentra.apiresource phone zentra.client zentra.user zentra.role zentra.identityresource zentra.adminuser zentra.securitytoken";

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

            var apiResources = ZentraMasterDataSeed.GetApiResourceEntityMaster();
            dbContext.ApiResources.AddRange(apiResources);

            var identityResources = ZentraMasterDataSeed.CreateIdentityResourceModelMaster();
            dbContext.IdentityResources.AddRange(identityResources);

            var roles = ZentraMasterDataSeed.CreateRolesMaster();
            dbContext.Roles.AddRange(roles);

            var securityQuestions = ZentraMasterDataSeed.CreateSecurityQuestionsModelMaster();
            dbContext.SecurityQuestions.AddRange(securityQuestions);

            var adminRole = roles.First(r => r.Name == "ZentraAdmin");
            var sfAdminClaims = ZentraMasterDataSeed.CreateRoleClaims_ZentraAdmin();
            foreach (var claim in sfAdminClaims) claim.RoleId = adminRole.Id;

            dbContext.RoleClaims.AddRange(sfAdminClaims);

            var userRole = roles.First(r => r.Name == "ZentraUser");
            var sfUserClaims = ZentraMasterDataSeed.CreateRoleClaims_ZentraUser();
            foreach (var claim in sfUserClaims) claim.RoleId = userRole.Id;

            dbContext.RoleClaims.AddRange(sfUserClaims);

            var rentflowOwnerRole = roles.First(r => r.Name == "rentflow_owner");
            var rentflowOwnerClaims = ZentraMasterDataSeed.CreateRoleClaims_RentFlowOwner();
            foreach (var claim in rentflowOwnerClaims) claim.RoleId = rentflowOwnerRole.Id;
            dbContext.RoleClaims.AddRange(rentflowOwnerClaims);

            var rentflowManagerRole = roles.First(r => r.Name == "rentflow_manager");
            var rentflowManagerClaims = ZentraMasterDataSeed.CreateRoleClaims_RentFlowManager();
            foreach (var claim in rentflowManagerClaims) claim.RoleId = rentflowManagerRole.Id;
            dbContext.RoleClaims.AddRange(rentflowManagerClaims);

            var rentflowResidentRole = roles.First(r => r.Name == "rentflow_resident");
            var rentflowResidentClaims = ZentraMasterDataSeed.CreateRoleClaims_RentFlowResident();
            foreach (var claim in rentflowResidentClaims) claim.RoleId = rentflowResidentRole.Id;
            dbContext.RoleClaims.AddRange(rentflowResidentClaims);

            var adminUser = BuildUser(seedConfiguration.AdminUser);
            dbContext.Users.Add(adminUser);

            var adminUserRole = ZentraMasterDataSeed.CreateUserRoleModelMaster();
            adminUserRole.RoleId = adminRole.Id;
            adminUserRole.UserId = adminUser.Id;
            dbContext.UserRoles.Add(adminUserRole);

            var standardUserRole = ZentraMasterDataSeed.CreateUserRoleModelMaster();
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
        var user = ZentraMasterDataSeed.CreateUserModelMaster();
        user.UserName = userConfiguration.UserName;
        user.NormalizedUserName = userConfiguration.UserName.ToUpperInvariant();
        user.Email = userConfiguration.Email;
        user.NormalizedEmail = userConfiguration.Email.ToUpperInvariant();
        user.PhoneNumber = userConfiguration.PhoneNumber;
        user.FirstName = userConfiguration.FirstName.Trim();
        user.LastName = userConfiguration.LastName?.Trim() ?? string.Empty;
        user.IdentityProviderType = userConfiguration.IdentityProvider;

        var hasher = new Argon2PasswordHasherWrapper<Users>();
        user.PasswordHash = hasher.HashPassword(user, userConfiguration.Password);

        return user;
    }

    private static (Clients Client, string GeneratedClientSecret) BuildClient(
        ClientConfigurationDto clientConfiguration)
    {
        var client = ZentraMasterDataSeed.CreateClientMaster();
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
                { RedirectUri = uri, CreatedOn = DateTime.UtcNow, CreatedBy = "ZentraUser" })
            .ToList();
        client.PostLogoutRedirectUris = clientConfiguration.PostLogoutRedirectUris
            .Select(uri => new ClientPostLogoutRedirectUris
                { PostLogoutRedirectUri = uri, CreatedOn = DateTime.UtcNow, CreatedBy = "ZentraUser" })
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
