/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;

namespace HCL.CS.DomainServices;

public interface IApplicationDbContext
{
    DbSet<AuditTrail> AuditTrail { get; set; }
    DbSet<SecurityQuestions> SecurityQuestions { get; set; }
    DbSet<UserSecurityQuestions> UserSecurityQuestions { get; set; }
    DbSet<PasswordHistory> PasswordHistory { get; set; }
    DbSet<Users> Users { get; set; }
    DbSet<UserClaims> UserClaims { get; set; }
    DbSet<UserLogins> UserLogins { get; set; }
    DbSet<UserTokens> UserTokens { get; set; }
    DbSet<Roles> Roles { get; set; }
    DbSet<RoleClaims> RoleClaims { get; set; }
    DbSet<UserRoles> UserRoles { get; set; }
    DbSet<ExternalIdentities> ExternalIdentities { get; set; }
    DbSet<Clients> Clients { get; set; }
    DbSet<Notification> Notification { get; set; }

    DbSet<ApiResources> ApiResources { get; set; }
    DbSet<ApiResourceClaims> ApiResourceClaims { get; set; }
    DbSet<ApiScopes> ApiScopes { get; set; }
    DbSet<ApiScopeClaims> ApiScopeClaims { get; set; }

    DbSet<IdentityResources> IdentityResources { get; set; }
    DbSet<IdentityClaims> IdentityClaims { get; set; }

    DbSet<SecurityTokens> SecurityTokens { get; set; }
    DbSet<T> Set<T>() where T : BaseEntity;

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
    void SetAddedStatus(object entry);
    void SetModifiedStatus(object entry);
    void SetPropertyModifiedStatus(object entry, string property);
    void SetConcurrencyOriginalValue(object entry, string dbConcurrencyStamp);
    void SetConcurrencyStatus(object entry, string dbConcurrencyStamp);
    void SetRowVersionStatus(object entry, byte[] dbRowVersionStamp);
}
