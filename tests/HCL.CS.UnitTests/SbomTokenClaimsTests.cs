/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
