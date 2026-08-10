/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants.Endpoint;

/// <summary>
/// Version 1 identity contract for the SBOM Analyzer. These values identify a user;
/// they do not represent SBOM tenant membership, roles, or permissions.
/// </summary>
public static class SbomIdentityContract
{
    public const int Version = 1;
    public const string ClientId = "sbom-analyser-web";
    public const string ApiAudience = "sbom-analyser-api";

    public static readonly IReadOnlySet<string> RequiredScopes =
        new HashSet<string>(StringComparer.Ordinal)
        {
            AuthenticationConstants.IdentityScopes.OpenId,
            AuthenticationConstants.IdentityScopes.Profile,
            AuthenticationConstants.IdentityScopes.Email,
            AuthenticationConstants.IdentityScopes.OfflineAccess,
            ApiAudience
        };

    public static readonly IReadOnlySet<string> RequiredUserClaims =
        new HashSet<string>(StringComparer.Ordinal)
        {
            ClaimNames.Subject,
            ClaimNames.Email,
            ClaimNames.Name,
            ClaimNames.PreferredUserName,
            ClaimNames.EmployeeId,
            ClaimNames.Department
        };

    public static class ClaimNames
    {
        public const string Subject = "sub";
        public const string Email = "email";
        public const string EmailVerified = "email_verified";
        public const string Name = "name";
        public const string PreferredUserName = "preferred_username";
        public const string EmployeeId = "employee_id";
        public const string Department = "department";
        public const string AuthenticationSource = "authentication_source";
    }
}
