using System.Security.Claims;
using FluentAssertions;
using HCL.CS.Domain.Models.Endpoint;
using Xunit;

namespace HCL.CS.UnitTests;

public class SbomTokenClaimsTests
{
    [Fact]
    public void AccessTokenClaims_ShouldIncludeRequestedIdentityAndTenantClaims()
    {
        var claims = new ResultClaimsModel
        {
            IdentityTokenScopeClaims = [new Claim("scope", "profile")],
            IdentityClaims = [new Claim("email", "user@example.local"), new Claim("name", "Test User")],
            RoleClaims = [new Claim("role", "SECURITY_ANALYST")],
            TransactionClaims = [],
            PermissionClaims = [],
            CustomAccessTokenClaims = [new Claim("tenant_id", "tenant-local")]
        };

        claims.AccessTokenClaims.Should().ContainSingle(x => x.Type == "email" && x.Value == "user@example.local");
        claims.AccessTokenClaims.Should().ContainSingle(x => x.Type == "name" && x.Value == "Test User");
        claims.AccessTokenClaims.Should().ContainSingle(x => x.Type == "role" && x.Value == "SECURITY_ANALYST");
        claims.AccessTokenClaims.Should().ContainSingle(x => x.Type == "tenant_id" && x.Value == "tenant-local");
    }
}
