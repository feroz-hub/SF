/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using FluentAssertions;
using HCL.CS.Domain.Configurations.Endpoint;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Service.Implementation.Api.Validators;
using HclCsInstallerMVC.Infrastructure.Seeding;
using HclCsInstallerMVC.Infrastructure.Services;
using System.Reflection;
using Xunit;

namespace HCL.CS.UnitTests;

/// <summary>
/// Guards the configuration contract between the Installer bootstrap Admin client and the backend/Admin
/// token-lifetime validation. A fresh Installer-created Admin client must be immediately editable in the
/// Admin UI without touching unrelated lifetime fields.
/// </summary>
public class InstallerAdminClientContractTests
{
    private static ClientsModel ToLifetimeModel(Clients seed)
    {
        return new ClientsModel
        {
            AccessTokenExpiration = seed.AccessTokenExpiration,
            RefreshTokenExpiration = seed.RefreshTokenExpiration,
            IdentityTokenExpiration = seed.IdentityTokenExpiration,
            LogoutTokenExpiration = seed.LogoutTokenExpiration,
            AuthorizationCodeExpiration = seed.AuthorizationCodeExpiration
        };
    }

    private static bool SatisfiesRange(ClientsModel model, string tokenType, TokenExpiration contract)
    {
        return tokenType switch
        {
            OpenIdConstants.TokenType.AccessToken => new CheckTokenExpirationRange<ClientsModel>(
                m => m.AccessTokenExpiration, tokenType, contract).IsSatisfiedBy(model),
            OpenIdConstants.TokenType.IdentityToken => new CheckTokenExpirationRange<ClientsModel>(
                m => m.IdentityTokenExpiration, tokenType, contract).IsSatisfiedBy(model),
            OpenIdConstants.TokenType.RefreshToken => new CheckTokenExpirationRange<ClientsModel>(
                m => m.RefreshTokenExpiration, tokenType, contract).IsSatisfiedBy(model),
            OpenIdConstants.TokenType.LogoutToken => new CheckTokenExpirationRange<ClientsModel>(
                m => m.LogoutTokenExpiration, tokenType, contract).IsSatisfiedBy(model),
            OpenIdConstants.TokenType.AuthorizationCode => new CheckTokenExpirationRange<ClientsModel>(
                m => m.AuthorizationCodeExpiration, tokenType, contract).IsSatisfiedBy(model),
            _ => false
        };
    }

    [Fact]
    public void InstallerAdminClientSeed_LifetimesAreWithinCanonicalContract()
    {
        var seed = HclCsMasterDataSeed.CreateClientMaster();
        var contract = new TokenExpiration();

        seed.AccessTokenExpiration.Should()
            .BeInRange(contract.MinAccessTokenExpiration, contract.MaxAccessTokenExpiration);
        seed.RefreshTokenExpiration.Should()
            .BeInRange(contract.MinRefreshTokenExpiration, contract.MaxRefreshTokenExpiration);
        seed.IdentityTokenExpiration.Should()
            .BeInRange(contract.MinIdentityTokenExpiration, contract.MaxIdentityTokenExpiration);
        seed.LogoutTokenExpiration.Should()
            .BeInRange(contract.MinLogoutTokenExpiration, contract.MaxLogoutTokenExpiration);
        seed.AuthorizationCodeExpiration.Should()
            .BeInRange(contract.MinAuthorizationCodeExpiration, contract.MaxAuthorizationCodeExpiration);
    }

    [Fact]
    public void InstallerAdminClientSeed_PassesBackendTokenExpirationValidators()
    {
        var model = ToLifetimeModel(HclCsMasterDataSeed.CreateClientMaster());
        var contract = new TokenExpiration();

        // Loading the seeded Admin client and submitting it (for example after adding
        // hcl-cs.client.manage) must not fail on any lifetime field.
        SatisfiesRange(model, OpenIdConstants.TokenType.AccessToken, contract).Should().BeTrue();
        SatisfiesRange(model, OpenIdConstants.TokenType.RefreshToken, contract).Should().BeTrue();
        SatisfiesRange(model, OpenIdConstants.TokenType.IdentityToken, contract).Should().BeTrue();
        SatisfiesRange(model, OpenIdConstants.TokenType.LogoutToken, contract).Should().BeTrue();
        SatisfiesRange(model, OpenIdConstants.TokenType.AuthorizationCode, contract).Should().BeTrue();
    }

    [Fact]
    public void InstallerAdminClientSeed_MatchesCanonicalClientDefaults()
    {
        // Pins the Installer seed to the domain ClientsModel defaults, which are also the Admin
        // "new client" form defaults. If any layer drifts, this fails.
        var seed = HclCsMasterDataSeed.CreateClientMaster();
        var canonical = new ClientsModel();

        seed.AccessTokenExpiration.Should().Be(canonical.AccessTokenExpiration).And.Be(900);
        seed.RefreshTokenExpiration.Should().Be(canonical.RefreshTokenExpiration).And.Be(86400);
        seed.IdentityTokenExpiration.Should().Be(canonical.IdentityTokenExpiration).And.Be(3600);
        seed.LogoutTokenExpiration.Should().Be(canonical.LogoutTokenExpiration).And.Be(1800);
        seed.AuthorizationCodeExpiration.Should().Be(canonical.AuthorizationCodeExpiration).And.Be(600);
    }

    [Theory]
    [InlineData(3600)] // the pre-fix seeded AccessTokenExpiration
    [InlineData(0)]
    [InlineData(901)]
    public void AccessTokenExpirationOutsideContract_IsRejectedByBackendValidator(int seconds)
    {
        var contract = new TokenExpiration();
        var model = new ClientsModel { AccessTokenExpiration = seconds };

        SatisfiesRange(model, OpenIdConstants.TokenType.AccessToken, contract).Should().BeFalse();
    }

    [Theory]
    [InlineData(1800)] // the pre-fix seeded AuthorizationCodeExpiration
    [InlineData(601)]
    public void AuthorizationCodeExpirationOutsideContract_IsRejectedByBackendValidator(int seconds)
    {
        var contract = new TokenExpiration();
        var model = new ClientsModel { AuthorizationCodeExpiration = seconds };

        SatisfiesRange(model, OpenIdConstants.TokenType.AuthorizationCode, contract).Should().BeFalse();
    }

    [Fact]
    public void CanonicalContractDefaults_MatchTheAdminUiRanges()
    {
        // The backend defaults are the authoritative contract; the Admin UI must mirror exactly these.
        var contract = new TokenExpiration();

        contract.MinAccessTokenExpiration.Should().Be(60);
        contract.MaxAccessTokenExpiration.Should().Be(900);
        contract.MinRefreshTokenExpiration.Should().Be(300);
        contract.MaxRefreshTokenExpiration.Should().Be(86400);
        contract.MinIdentityTokenExpiration.Should().Be(60);
        contract.MaxIdentityTokenExpiration.Should().Be(3600);
        contract.MinLogoutTokenExpiration.Should().Be(1800);
        contract.MaxLogoutTokenExpiration.Should().Be(86400);
        contract.MinAuthorizationCodeExpiration.Should().Be(60);
        contract.MaxAuthorizationCodeExpiration.Should().Be(600);
    }

    [Fact]
    public void InstallerDefaultAdminScopes_MatchSeededGranularScopeContract()
    {
        var defaultScopesField = typeof(SeedDataService).GetField(
            "DefaultScopes",
            BindingFlags.NonPublic | BindingFlags.Static);
        var installerDefaultScopes = ((string)defaultScopesField!.GetValue(null)!)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        installerDefaultScopes.Should().Equal(AdminClientScopeContract.AllScopes);
        installerDefaultScopes.Should().OnlyHaveUniqueItems();

        var seededPermissionScopes = HclCsMasterDataSeed.GetApiResourceEntityMaster()
            .SelectMany(resource => resource.ApiScopes)
            .Select(scope => scope.Name)
            .ToArray();
        AdminClientScopeContract.PermissionScopes.Should().BeEquivalentTo(seededPermissionScopes);

        var persistedIdentityScopes = HclCsMasterDataSeed.CreateIdentityResourceModelMaster()
            .Select(resource => resource.Name)
            .ToHashSet(StringComparer.Ordinal);
        AdminClientScopeContract.IdentityScopes
            .Where(scope => scope != AuthenticationConstants.IdentityScopes.OfflineAccess)
            .Should().OnlyContain(scope => persistedIdentityScopes.Contains(scope));
        AdminClientScopeContract.IdentityScopes.Should()
            .Contain(AuthenticationConstants.IdentityScopes.OfflineAccess);

        var obsoleteUmbrellaScopes = HclCsMasterDataSeed.GetApiResourceEntityMaster()
            .Select(resource => resource.Name);
        installerDefaultScopes.Intersect(obsoleteUmbrellaScopes, StringComparer.Ordinal)
            .Should().BeEmpty();
    }
}
