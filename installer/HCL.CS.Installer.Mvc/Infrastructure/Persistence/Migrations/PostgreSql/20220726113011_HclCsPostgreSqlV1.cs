using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HclCsInstallerMVC.Infrastructure.Persistence.Migrations.PostgreSql;

public partial class HclCsPostgreSqlV1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "HclCs_ApiResources",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                Description = table.Column<string>(nullable: true),
                Enabled = table.Column<bool>(nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_ApiResources", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_AuditTrail",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ActionType = table.Column<int>(maxLength: 50, nullable: false),
                TableName = table.Column<string>(maxLength: 255, nullable: true),
                OldValue = table.Column<string>(nullable: true),
                NewValue = table.Column<string>(nullable: true),
                AffectedColumn = table.Column<string>(nullable: true),
                ActionName = table.Column<string>(nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_AuditTrail", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_Clients",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                ClientId = table.Column<string>(maxLength: 128, nullable: false),
                ClientName = table.Column<string>(maxLength: 255, nullable: true),
                ClientUri = table.Column<string>(nullable: true),
                ClientIdIssuedAt = table.Column<long>(nullable: false),
                ClientSecretExpiresAt = table.Column<long>(nullable: false),
                ClientSecret = table.Column<string>(maxLength: 128, nullable: true),
                LogoUri = table.Column<string>(nullable: true),
                TermsOfServiceUri = table.Column<string>(nullable: true),
                PolicyUri = table.Column<string>(nullable: true),
                RefreshTokenExpiration = table.Column<int>(nullable: false),
                AccessTokenExpiration = table.Column<int>(nullable: false),
                IdentityTokenExpiration = table.Column<int>(nullable: false),
                LogoutTokenExpiration = table.Column<int>(nullable: false),
                AuthorizationCodeExpiration = table.Column<int>(nullable: false),
                AccessTokenType = table.Column<int>(nullable: false),
                RequirePkce = table.Column<bool>(nullable: false),
                IsPkceTextPlain = table.Column<bool>(nullable: false),
                RequireClientSecret = table.Column<bool>(nullable: false),
                IsFirstPartyApp = table.Column<bool>(nullable: false),
                AllowOfflineAccess = table.Column<bool>(nullable: false),
                AllowedScopes = table.Column<string>(nullable: true),
                AllowAccessTokensViaBrowser = table.Column<bool>(nullable: false),
                ApplicationType = table.Column<int>(nullable: false),
                AllowedSigningAlgorithm = table.Column<string>(nullable: true),
                SupportedGrantTypes = table.Column<string>(nullable: true),
                SupportedResponseTypes = table.Column<string>(nullable: true),
                FrontChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                FrontChannelLogoutUri = table.Column<string>(nullable: true),
                BackChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                BackChannelLogoutUri = table.Column<string>(nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_Clients", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_IdentityResources",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                Description = table.Column<string>(nullable: true),
                Enabled = table.Column<bool>(nullable: false),
                Required = table.Column<bool>(nullable: false),
                Emphasize = table.Column<bool>(nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_IdentityResources", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_Roles",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                NormalizedName = table.Column<string>(maxLength: 255, nullable: false),
                ConcurrencyStamp = table.Column<string>(maxLength: 255, nullable: false),
                Description = table.Column<string>(nullable: true),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_Roles", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_SecurityQuestions",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                Question = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_SecurityQuestions", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_SecurityTokens",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                Key = table.Column<string>(nullable: true),
                TokenType = table.Column<string>(nullable: true),
                TokenValue = table.Column<string>(nullable: true),
                ClientId = table.Column<string>(nullable: true),
                SessionId = table.Column<string>(nullable: true),
                SubjectId = table.Column<string>(nullable: true),
                CreationTime = table.Column<DateTime>(nullable: false),
                ExpiresAt = table.Column<int>(nullable: false),
                ConsumedTime = table.Column<DateTime>(nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_SecurityTokens", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_Users",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                UserName = table.Column<string>(maxLength: 255, nullable: false),
                NormalizedUserName = table.Column<string>(maxLength: 255, nullable: false),
                Email = table.Column<string>(maxLength: 255, nullable: false),
                NormalizedEmail = table.Column<string>(maxLength: 255, nullable: false),
                EmailConfirmed = table.Column<bool>(nullable: false),
                PasswordHash = table.Column<string>(nullable: false),
                SecurityStamp = table.Column<string>(maxLength: 255, nullable: true),
                ConcurrencyStamp = table.Column<string>(maxLength: 255, nullable: true),
                PhoneNumber = table.Column<string>(maxLength: 15, nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                TwoFactorEnabled = table.Column<bool>(nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                LockoutEnabled = table.Column<bool>(nullable: false),
                AccessFailedCount = table.Column<int>(nullable: false),
                FirstName = table.Column<string>(maxLength: 255, nullable: false),
                LastName = table.Column<string>(maxLength: 255, nullable: true),
                DateOfBirth = table.Column<DateTime>(nullable: true),
                TwoFactorType = table.Column<int>(nullable: false),
                LastPasswordChangedDate = table.Column<DateTime>(nullable: true),
                RequiresDefaultPasswordChange = table.Column<bool>(nullable: true),
                LastLoginDateTime = table.Column<DateTime>(nullable: true),
                LastLogoutDateTime = table.Column<DateTime>(nullable: true),
                IdentityProviderType = table.Column<int>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_HclCs_Users", x => x.Id); });

        migrationBuilder.CreateTable(
            "HclCs_ApiResourceClaims",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                ApiResourceId = table.Column<Guid>(nullable: false),
                Type = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ApiResourceClaims", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_ApiResourceClaims_HclCs_ApiResources_ApiResourceId",
                    x => x.ApiResourceId,
                    "HclCs_ApiResources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "HclCs_ApiScopes",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                ApiResourceId = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                Description = table.Column<string>(nullable: true),
                Required = table.Column<bool>(nullable: false),
                Emphasize = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ApiScopes", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_ApiScopes_HclCs_ApiResources_ApiResourceId",
                    x => x.ApiResourceId,
                    "HclCs_ApiResources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "HclCs_ClientPostLogoutRedirectUris",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                ClientId = table.Column<Guid>(nullable: false),
                PostLogoutRedirectUri = table.Column<string>(maxLength: 510, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ClientPostLogoutRedirectUris", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_ClientPostLogoutRedirectUris_HclCs_Clients_ClientId",
                    x => x.ClientId,
                    "HclCs_Clients",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "HclCs_ClientRedirectUris",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                ClientId = table.Column<Guid>(nullable: false),
                RedirectUri = table.Column<string>(maxLength: 510, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ClientRedirectUris", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_ClientRedirectUris_HclCs_Clients_ClientId",
                    x => x.ClientId,
                    "HclCs_Clients",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "HclCs_IdentityClaims",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                IdentityResourceId = table.Column<Guid>(nullable: false),
                Type = table.Column<string>(maxLength: 255, nullable: false),
                AliasType = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_IdentityClaims", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_IdentityClaims_HclCs_IdentityResources_IdentityResourceId",
                    x => x.IdentityResourceId,
                    "HclCs_IdentityResources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "HclCs_RoleClaims",
            table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<Guid>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_RoleClaims", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_RoleClaims_HclCs_Roles_RoleId",
                    x => x.RoleId,
                    "HclCs_Roles",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_Notification",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                UserId = table.Column<Guid>(nullable: false),
                MessageId = table.Column<string>(maxLength: 255, nullable: false),
                Type = table.Column<int>(nullable: false),
                Activity = table.Column<string>(maxLength: 255, nullable: true),
                Status = table.Column<int>(maxLength: 255, nullable: false),
                Sender = table.Column<string>(maxLength: 255, nullable: false),
                Recipient = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_Notification", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_Notification_HclCs_Users_UserId",
                    x => x.UserId,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_PasswordHistory",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                UserID = table.Column<Guid>(nullable: false),
                ChangedOn = table.Column<DateTime>(nullable: false),
                PasswordHash = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_PasswordHistory", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_PasswordHistory_HclCs_Users_UserID",
                    x => x.UserID,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_UserClaims",
            table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<Guid>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true),
                IsAdminClaim = table.Column<bool>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_UserClaims", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_UserClaims_HclCs_Users_UserId",
                    x => x.UserId,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_UserLogins",
            table => new
            {
                LoginProvider = table.Column<string>(maxLength: 256, nullable: false),
                ProviderKey = table.Column<string>(maxLength: 256, nullable: false),
                UserId = table.Column<Guid>(nullable: false),
                ProviderDisplayName = table.Column<string>(nullable: true),
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_UserLogins", x => new { x.LoginProvider, x.ProviderKey, x.UserId });
                table.ForeignKey(
                    "FK_HclCs_UserLogins_HclCs_Users_UserId",
                    x => x.UserId,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_UserRoles",
            table => new
            {
                UserId = table.Column<Guid>(nullable: false),
                RoleId = table.Column<Guid>(nullable: false),
                Id = table.Column<Guid>(nullable: false),
                ValidFrom = table.Column<DateTime>(nullable: true),
                ValidTo = table.Column<DateTime>(nullable: true),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_UserRoles", x => new { x.Id, x.UserId, x.RoleId });
                table.ForeignKey(
                    "FK_HclCs_UserRoles_HclCs_Roles_RoleId",
                    x => x.RoleId,
                    "HclCs_Roles",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    "FK_HclCs_UserRoles_HclCs_Users_UserId",
                    x => x.UserId,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_UserSecurityQuestions",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                UserId = table.Column<Guid>(nullable: false),
                SecurityQuestionId = table.Column<Guid>(nullable: false),
                Answer = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_UserSecurityQuestions", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_UserSecurityQuestions_HclCs_SecurityQuestions_SecurityQuest~",
                    x => x.SecurityQuestionId,
                    "HclCs_SecurityQuestions",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    "FK_HclCs_UserSecurityQuestions_HclCs_Users_UserId",
                    x => x.UserId,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_UserTokens",
            table => new
            {
                UserId = table.Column<Guid>(nullable: false),
                LoginProvider = table.Column<string>(maxLength: 255, nullable: false),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                Value = table.Column<string>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    "FK_HclCs_UserTokens_HclCs_Users_UserId",
                    x => x.UserId,
                    "HclCs_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "HclCs_ApiScopeClaims",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                xmin = table.Column<uint>("xid", rowVersion: true, nullable: true),
                ApiScopeId = table.Column<Guid>(nullable: false),
                Type = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ApiScopeClaims", x => x.Id);
                table.ForeignKey(
                    "FK_HclCs_ApiScopeClaims_HclCs_ApiScopes_ApiScopeId",
                    x => x.ApiScopeId,
                    "HclCs_ApiScopes",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_APIRES_CLM_RESID_TYPE",
            "HclCs_ApiResourceClaims",
            new[] { "ApiResourceId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_APIRES_NAME",
            "HclCs_ApiResources",
            "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_APISCO_CLM_SCOID_TYPE",
            "HclCs_ApiScopeClaims",
            new[] { "ApiScopeId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_APISCO_SCOID_NAME",
            "HclCs_ApiScopes",
            new[] { "ApiResourceId", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_AUD_CBBY_ACTY",
            "HclCs_AuditTrail",
            new[] { "CreatedBy", "ActionType" });

        migrationBuilder.CreateIndex(
            "IX_AUD_CRON_ACTY",
            "HclCs_AuditTrail",
            new[] { "CreatedOn", "ActionType" });

        migrationBuilder.CreateIndex(
            "IX_AUD_CRON_CBBY",
            "HclCs_AuditTrail",
            new[] { "CreatedOn", "CreatedBy" });

        migrationBuilder.CreateIndex(
            "IX_HclCs_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirect~",
            "HclCs_ClientPostLogoutRedirectUris",
            new[] { "ClientId", "PostLogoutRedirectUri" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_HclCs_ClientRedirectUris_ClientId_RedirectUri",
            "HclCs_ClientRedirectUris",
            new[] { "ClientId", "RedirectUri" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_CLI_CLID_CLSEC",
            "HclCs_Clients",
            new[] { "ClientId", "ClientSecret" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_IDRESCLM_IDRESID_TYPE",
            "HclCs_IdentityClaims",
            new[] { "IdentityResourceId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_IDRES_NAME",
            "HclCs_IdentityResources",
            "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_NOTI_TYPE",
            "HclCs_Notification",
            "Type");

        migrationBuilder.CreateIndex(
            "IX_HclCs_Notification_UserId",
            "HclCs_Notification",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_HclCs_PasswordHistory_UserID",
            "HclCs_PasswordHistory",
            "UserID");

        migrationBuilder.CreateIndex(
            "IX_HclCs_RoleClaims_RoleId",
            "HclCs_RoleClaims",
            "RoleId");

        migrationBuilder.CreateIndex(
            "RoleNameIndex",
            "HclCs_Roles",
            "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_SEC_QUESTION",
            "HclCs_SecurityQuestions",
            "Question",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_HclCs_UserClaims_UserId",
            "HclCs_UserClaims",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_HclCs_UserLogins_UserId",
            "HclCs_UserLogins",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_HclCs_UserRoles_RoleId",
            "HclCs_UserRoles",
            "RoleId");

        migrationBuilder.CreateIndex(
            "IX_HclCs_UserRoles_UserId",
            "HclCs_UserRoles",
            "UserId");

        migrationBuilder.CreateIndex(
            "EmailIndex",
            "HclCs_Users",
            "NormalizedEmail");

        migrationBuilder.CreateIndex(
            "UserNameIndex",
            "HclCs_Users",
            "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_USRSEC_QUEID",
            "HclCs_UserSecurityQuestions",
            "SecurityQuestionId");

        migrationBuilder.CreateIndex(
            "IX_USRSEC_UID_QUEID",
            "HclCs_UserSecurityQuestions",
            new[] { "UserId", "SecurityQuestionId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "HclCs_ApiResourceClaims");

        migrationBuilder.DropTable(
            "HclCs_ApiScopeClaims");

        migrationBuilder.DropTable(
            "HclCs_AuditTrail");

        migrationBuilder.DropTable(
            "HclCs_ClientPostLogoutRedirectUris");

        migrationBuilder.DropTable(
            "HclCs_ClientRedirectUris");

        migrationBuilder.DropTable(
            "HclCs_IdentityClaims");

        migrationBuilder.DropTable(
            "HclCs_Notification");

        migrationBuilder.DropTable(
            "HclCs_PasswordHistory");

        migrationBuilder.DropTable(
            "HclCs_RoleClaims");

        migrationBuilder.DropTable(
            "HclCs_SecurityTokens");

        migrationBuilder.DropTable(
            "HclCs_UserClaims");

        migrationBuilder.DropTable(
            "HclCs_UserLogins");

        migrationBuilder.DropTable(
            "HclCs_UserRoles");

        migrationBuilder.DropTable(
            "HclCs_UserSecurityQuestions");

        migrationBuilder.DropTable(
            "HclCs_UserTokens");

        migrationBuilder.DropTable(
            "HclCs_ApiScopes");

        migrationBuilder.DropTable(
            "HclCs_Clients");

        migrationBuilder.DropTable(
            "HclCs_IdentityResources");

        migrationBuilder.DropTable(
            "HclCs_Roles");

        migrationBuilder.DropTable(
            "HclCs_SecurityQuestions");

        migrationBuilder.DropTable(
            "HclCs_Users");

        migrationBuilder.DropTable(
            "HclCs_ApiResources");
    }
}
