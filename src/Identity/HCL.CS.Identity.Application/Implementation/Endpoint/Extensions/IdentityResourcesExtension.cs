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
}
