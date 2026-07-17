using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZentraInstallerMVC.Infrastructure.Persistence.Migrations.MySql;

public partial class ZentraMySqlV1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Zentra_ApiResources",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                Description = table.Column<string>(nullable: true),
                Enabled = table.Column<bool>(nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Zentra_ApiResources", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_AuditTrail",
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
            constraints: table => { table.PrimaryKey("PK_Zentra_AuditTrail", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_Clients",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
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
            constraints: table => { table.PrimaryKey("PK_Zentra_Clients", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_IdentityResources",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                Description = table.Column<string>(nullable: true),
                Enabled = table.Column<bool>(nullable: false),
                Required = table.Column<bool>(nullable: false),
                Emphasize = table.Column<bool>(nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Zentra_IdentityResources", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_Roles",
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
            constraints: table => { table.PrimaryKey("PK_Zentra_Roles", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_SecurityQuestions",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                Question = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Zentra_SecurityQuestions", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_SecurityTokens",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
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
            constraints: table => { table.PrimaryKey("PK_Zentra_SecurityTokens", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_Users",
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
            constraints: table => { table.PrimaryKey("PK_Zentra_Users", x => x.Id); });

        migrationBuilder.CreateTable(
            "Zentra_ApiResourceClaims",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                ApiResourceId = table.Column<Guid>(nullable: false),
                Type = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_ApiResourceClaims", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_ApiResourceClaims_Zentra_ApiResources_ApiResourceId",
                    x => x.ApiResourceId,
                    "Zentra_ApiResources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Zentra_ApiScopes",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                ApiResourceId = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                Description = table.Column<string>(nullable: true),
                Required = table.Column<bool>(nullable: false),
                Emphasize = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_ApiScopes", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_ApiScopes_Zentra_ApiResources_ApiResourceId",
                    x => x.ApiResourceId,
                    "Zentra_ApiResources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Zentra_ClientPostLogoutRedirectUris",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                ClientId = table.Column<Guid>(nullable: false),
                PostLogoutRedirectUri = table.Column<string>(maxLength: 510, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_ClientPostLogoutRedirectUris", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_ClientPostLogoutRedirectUris_Zentra_Clients_ClientId",
                    x => x.ClientId,
                    "Zentra_Clients",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Zentra_ClientRedirectUris",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                ClientId = table.Column<Guid>(nullable: false),
                RedirectUri = table.Column<string>(maxLength: 510, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_ClientRedirectUris", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_ClientRedirectUris_Zentra_Clients_ClientId",
                    x => x.ClientId,
                    "Zentra_Clients",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Zentra_IdentityClaims",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                IdentityResourceId = table.Column<Guid>(nullable: false),
                Type = table.Column<string>(maxLength: 255, nullable: false),
                AliasType = table.Column<string>(maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_IdentityClaims", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_IdentityClaims_Zentra_IdentityResources_IdentityResourceId",
                    x => x.IdentityResourceId,
                    "Zentra_IdentityResources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Zentra_RoleClaims",
            table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                RoleId = table.Column<Guid>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_RoleClaims", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_RoleClaims_Zentra_Roles_RoleId",
                    x => x.RoleId,
                    "Zentra_Roles",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_Notification",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
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
                table.PrimaryKey("PK_Zentra_Notification", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_Notification_Zentra_Users_UserId",
                    x => x.UserId,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_PasswordHistory",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                UserID = table.Column<Guid>(nullable: false),
                ChangedOn = table.Column<DateTime>(nullable: false),
                PasswordHash = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_PasswordHistory", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_PasswordHistory_Zentra_Users_UserID",
                    x => x.UserID,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_UserClaims",
            table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                UserId = table.Column<Guid>(nullable: false),
                ClaimType = table.Column<string>(nullable: true),
                ClaimValue = table.Column<string>(nullable: true),
                IsAdminClaim = table.Column<bool>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_UserClaims", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_UserClaims_Zentra_Users_UserId",
                    x => x.UserId,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_UserLogins",
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
                table.PrimaryKey("PK_Zentra_UserLogins", x => new { x.LoginProvider, x.ProviderKey, x.UserId });
                table.ForeignKey(
                    "FK_Zentra_UserLogins_Zentra_Users_UserId",
                    x => x.UserId,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_UserRoles",
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
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_UserRoles", x => new { x.Id, x.UserId, x.RoleId });
                table.ForeignKey(
                    "FK_Zentra_UserRoles_Zentra_Roles_RoleId",
                    x => x.RoleId,
                    "Zentra_Roles",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    "FK_Zentra_UserRoles_Zentra_Users_UserId",
                    x => x.UserId,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_UserSecurityQuestions",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                UserId = table.Column<Guid>(nullable: false),
                SecurityQuestionId = table.Column<Guid>(nullable: false),
                Answer = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_UserSecurityQuestions", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_UserSecurityQuestions_Zentra_SecurityQuestions_SecurityQuesti~",
                    x => x.SecurityQuestionId,
                    "Zentra_SecurityQuestions",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    "FK_Zentra_UserSecurityQuestions_Zentra_Users_UserId",
                    x => x.UserId,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_UserTokens",
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
                table.PrimaryKey("PK_Zentra_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    "FK_Zentra_UserTokens_Zentra_Users_UserId",
                    x => x.UserId,
                    "Zentra_Users",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Zentra_ApiScopeClaims",
            table => new
            {
                Id = table.Column<Guid>(nullable: false),
                IsDeleted = table.Column<bool>(nullable: false),
                CreatedOn = table.Column<DateTime>(nullable: false),
                ModifiedOn = table.Column<DateTime>(nullable: true),
                CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                ApiScopeId = table.Column<Guid>(nullable: false),
                Type = table.Column<string>(maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Zentra_ApiScopeClaims", x => x.Id);
                table.ForeignKey(
                    "FK_Zentra_ApiScopeClaims_Zentra_ApiScopes_ApiScopeId",
                    x => x.ApiScopeId,
                    "Zentra_ApiScopes",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_APIRES_CLM_RESID_TYPE",
            "Zentra_ApiResourceClaims",
            new[] { "ApiResourceId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_APIRES_NAME",
            "Zentra_ApiResources",
            "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_APISCO_CLM_SCOID_TYPE",
            "Zentra_ApiScopeClaims",
            new[] { "ApiScopeId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_APISCO_SCOID_NAME",
            "Zentra_ApiScopes",
            new[] { "ApiResourceId", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_AUD_CBBY_ACTY",
            "Zentra_AuditTrail",
            new[] { "CreatedBy", "ActionType" });

        migrationBuilder.CreateIndex(
            "IX_AUD_CRON_ACTY",
            "Zentra_AuditTrail",
            new[] { "CreatedOn", "ActionType" });

        migrationBuilder.CreateIndex(
            "IX_AUD_CRON_CBBY",
            "Zentra_AuditTrail",
            new[] { "CreatedOn", "CreatedBy" });

        migrationBuilder.CreateIndex(
            "IX_Zentra_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectU~",
            "Zentra_ClientPostLogoutRedirectUris",
            new[] { "ClientId", "PostLogoutRedirectUri" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Zentra_ClientRedirectUris_ClientId_RedirectUri",
            "Zentra_ClientRedirectUris",
            new[] { "ClientId", "RedirectUri" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_CLI_CLID_CLSEC",
            "Zentra_Clients",
            new[] { "ClientId", "ClientSecret" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_IDRESCLM_IDRESID_TYPE",
            "Zentra_IdentityClaims",
            new[] { "IdentityResourceId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_IDRES_NAME",
            "Zentra_IdentityResources",
            "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_NOTI_TYPE",
            "Zentra_Notification",
            "Type");

        migrationBuilder.CreateIndex(
            "IX_Zentra_Notification_UserId",
            "Zentra_Notification",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Zentra_PasswordHistory_UserID",
            "Zentra_PasswordHistory",
            "UserID");

        migrationBuilder.CreateIndex(
            "IX_Zentra_RoleClaims_RoleId",
            "Zentra_RoleClaims",
            "RoleId");

        migrationBuilder.CreateIndex(
            "RoleNameIndex",
            "Zentra_Roles",
            "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_SEC_QUESTION",
            "Zentra_SecurityQuestions",
            "Question",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Zentra_UserClaims_UserId",
            "Zentra_UserClaims",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Zentra_UserLogins_UserId",
            "Zentra_UserLogins",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Zentra_UserRoles_RoleId",
            "Zentra_UserRoles",
            "RoleId");

        migrationBuilder.CreateIndex(
            "IX_Zentra_UserRoles_UserId",
            "Zentra_UserRoles",
            "UserId");

        migrationBuilder.CreateIndex(
            "EmailIndex",
            "Zentra_Users",
            "NormalizedEmail");

        migrationBuilder.CreateIndex(
            "UserNameIndex",
            "Zentra_Users",
            "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_USRSEC_QUEID",
            "Zentra_UserSecurityQuestions",
            "SecurityQuestionId");

        migrationBuilder.CreateIndex(
            "IX_USRSEC_UID_QUEID",
            "Zentra_UserSecurityQuestions",
            new[] { "UserId", "SecurityQuestionId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Zentra_ApiResourceClaims");

        migrationBuilder.DropTable(
            "Zentra_ApiScopeClaims");

        migrationBuilder.DropTable(
            "Zentra_AuditTrail");

        migrationBuilder.DropTable(
            "Zentra_ClientPostLogoutRedirectUris");

        migrationBuilder.DropTable(
            "Zentra_ClientRedirectUris");

        migrationBuilder.DropTable(
            "Zentra_IdentityClaims");

        migrationBuilder.DropTable(
            "Zentra_Notification");

        migrationBuilder.DropTable(
            "Zentra_PasswordHistory");

        migrationBuilder.DropTable(
            "Zentra_RoleClaims");

        migrationBuilder.DropTable(
            "Zentra_SecurityTokens");

        migrationBuilder.DropTable(
            "Zentra_UserClaims");

        migrationBuilder.DropTable(
            "Zentra_UserLogins");

        migrationBuilder.DropTable(
            "Zentra_UserRoles");

        migrationBuilder.DropTable(
            "Zentra_UserSecurityQuestions");

        migrationBuilder.DropTable(
            "Zentra_UserTokens");

        migrationBuilder.DropTable(
            "Zentra_ApiScopes");

        migrationBuilder.DropTable(
            "Zentra_Clients");

        migrationBuilder.DropTable(
            "Zentra_IdentityResources");

        migrationBuilder.DropTable(
            "Zentra_Roles");

        migrationBuilder.DropTable(
            "Zentra_SecurityQuestions");

        migrationBuilder.DropTable(
            "Zentra_Users");

        migrationBuilder.DropTable(
            "Zentra_ApiResources");
    }
}
