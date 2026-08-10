/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Reflection;
using System.Security.Claims;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Implementation.Endpoint.Extensions;

public static class IdentityResourceExtension
{
    public static async Task<List<Claim>> GetUserIdentityResources(this UserModel userInfo,
        IList<IdentityResourcesByScopesModel> identityResources)
    {
        var userClaimsList = new List<string>();
        var resultClaims = new List<Claim>();

        var userClaims = userInfo.UserClaims;
        var userInfoProperties = userInfo.GetType().GetProperties();
        var userInfoPropertyNames = userInfoProperties.ToList().ConvertAll(property => property.Name.ToLower());
        if (userInfo.UserClaims.ContainsAny())
            userClaimsList.AddRange(userClaims.ConvertAll(userClaim => userClaim.ClaimType.ToLower()));

        foreach (var idResource in identityResources)
        {
            var claimType = !string.IsNullOrWhiteSpace(idResource.IdentityResourceClaimAliasType)
                ? idResource.IdentityResourceClaimAliasType
                : idResource.IdentityResourceClaimType;
            object infoValue = null;
            if (userClaimsList.Contains(claimType))
                infoValue = userClaims.FirstOrDefault(userClaim => userClaim.ClaimType.ToLower() == claimType)
                    ?.ClaimValue;
            else if (userInfoPropertyNames.Contains(claimType))
                infoValue = userInfo.GetType().GetProperty(
                        claimType,
                        BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public |
                        BindingFlags.Instance)
                    ?.GetValue(userInfo, null);

            infoValue ??= ResolveIdentityFallback(userInfo, claimType);
            if (infoValue != null)
            {
                var stdClaim = AuthenticationConstants.StandardClaims.FirstOrDefault(x => x.Key == claimType);
                if (stdClaim.Value != null)
                    resultClaims.Add(new Claim(stdClaim.Value, infoValue.ToString()));
                else
                    resultClaims.Add(new Claim(claimType, infoValue.ToString()));
            }
        }

        return await Task.FromResult(resultClaims);
    }

    public static IEnumerable<Claim> NormalizeSbomProtocolClaims(this IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        return claims.Select(claim =>
        {
            var claimType = claim.Type switch
            {
                System.Security.Claims.ClaimTypes.Email => SbomIdentityContract.ClaimNames.Email,
                "displayname" => SbomIdentityContract.ClaimNames.Name,
                "userprincipalname" => SbomIdentityContract.ClaimNames.PreferredUserName,
                "employeeid" => SbomIdentityContract.ClaimNames.EmployeeId,
                "emailconfirmed" => SbomIdentityContract.ClaimNames.EmailVerified,
                _ => claim.Type
            };
            var value = claimType == SbomIdentityContract.ClaimNames.EmailVerified
                ? claim.Value.ToLowerInvariant()
                : claim.Value;
            var valueType = claimType == SbomIdentityContract.ClaimNames.EmailVerified
                ? ClaimValueTypes.Boolean
                : claim.ValueType;
            return new Claim(claimType, value, valueType);
        });
    }

    private static object? ResolveIdentityFallback(UserModel userInfo, string claimType)
    {
        if (string.Equals(
                claimType,
                nameof(UserModel.DisplayName),
                StringComparison.OrdinalIgnoreCase))
        {
            var fullName = string.Join(
                " ",
                new[] { userInfo.FirstName, userInfo.LastName }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));
            return string.IsNullOrWhiteSpace(fullName) ? userInfo.UserName : fullName;
        }

        if (string.Equals(
                claimType,
                nameof(UserModel.UserPrincipalName),
                StringComparison.OrdinalIgnoreCase))
            return userInfo.UserName;

        return null;
    }
}
