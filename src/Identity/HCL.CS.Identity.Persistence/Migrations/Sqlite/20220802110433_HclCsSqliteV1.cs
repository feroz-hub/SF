using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HCL.CS.Infrastructure.Data.Migrations.Sqlite
{
    public partial class HclCsSqliteV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HclCs_ApiResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_ApiResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_AuditTrail",
                columns: table => new
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_IdentityResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    Required = table.Column<bool>(nullable: false),
                    Emphasize = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_IdentityResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_Roles",
                columns: table => new
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_SecurityQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Question = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_SecurityQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_SecurityTokens",
                columns: table => new
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_SecurityTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_Users",
                columns: table => new
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_ApiResourceClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ApiResourceId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_ApiResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_ApiResourceClaims_HclCs_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "HclCs_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_ApiScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
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
                        name: "FK_HclCs_ApiScopes_HclCs_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "HclCs_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_ClientPostLogoutRedirectUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ClientId = table.Column<Guid>(nullable: false),
                    PostLogoutRedirectUri = table.Column<string>(maxLength: 510, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_ClientPostLogoutRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_ClientPostLogoutRedirectUris_HclCs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "HclCs_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_ClientRedirectUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ClientId = table.Column<Guid>(nullable: false),
                    RedirectUri = table.Column<string>(maxLength: 510, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_ClientRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_ClientRedirectUris_HclCs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "HclCs_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_IdentityClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IdentityResourceId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 255, nullable: false),
                    AliasType = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_IdentityClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_IdentityClaims_HclCs_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "HclCs_IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_RoleClaims_HclCs_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "HclCs_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_Notification",
                columns: table => new
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
                    table.PrimaryKey("PK_HclCs_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_Notification_HclCs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_PasswordHistory",
                columns: table => new
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
                    table.PrimaryKey("PK_HclCs_PasswordHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_PasswordHistory_HclCs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    IsAdminClaim = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_UserClaims_HclCs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_UserLogins",
                columns: table => new
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
                        name: "FK_HclCs_UserLogins_HclCs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_UserRoles",
                columns: table => new
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
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_UserRoles", x => new { x.Id, x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_HclCs_UserRoles_HclCs_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "HclCs_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HclCs_UserRoles_HclCs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_UserSecurityQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    SecurityQuestionId = table.Column<Guid>(nullable: false),
                    Answer = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_UserSecurityQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_UserSecurityQuestions_HclCs_SecurityQuestions_SecurityQuestionId",
                        column: x => x.SecurityQuestionId,
                        principalTable: "HclCs_SecurityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HclCs_UserSecurityQuestions_HclCs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_UserTokens",
                columns: table => new
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
                        name: "FK_HclCs_UserTokens_HclCs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "HclCs_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HclCs_ApiScopeClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 255, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ApiScopeId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HclCs_ApiScopeClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HclCs_ApiScopeClaims_HclCs_ApiScopes_ApiScopeId",
                        column: x => x.ApiScopeId,
                        principalTable: "HclCs_ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APIRES_CLM_RESID_TYPE",
                table: "HclCs_ApiResourceClaims",
                columns: new[] { "ApiResourceId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APIRES_NAME",
                table: "HclCs_ApiResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APISCO_CLM_SCOID_TYPE",
                table: "HclCs_ApiScopeClaims",
                columns: new[] { "ApiScopeId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APISCO_SCOID_NAME",
                table: "HclCs_ApiScopes",
                columns: new[] { "ApiResourceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUD_CBBY_ACTY",
                table: "HclCs_AuditTrail",
                columns: new[] { "CreatedBy", "ActionType" });

            migrationBuilder.CreateIndex(
                name: "IX_AUD_CRON_ACTY",
                table: "HclCs_AuditTrail",
                columns: new[] { "CreatedOn", "ActionType" });

            migrationBuilder.CreateIndex(
                name: "IX_AUD_CRON_CBBY",
                table: "HclCs_AuditTrail",
                columns: new[] { "CreatedOn", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri",
                table: "HclCs_ClientPostLogoutRedirectUris",
                columns: new[] { "ClientId", "PostLogoutRedirectUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_ClientRedirectUris_ClientId_RedirectUri",
                table: "HclCs_ClientRedirectUris",
                columns: new[] { "ClientId", "RedirectUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLI_CLID_CLSEC",
                table: "HclCs_Clients",
                columns: new[] { "ClientId", "ClientSecret" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IDRESCLM_IDRESID_TYPE",
                table: "HclCs_IdentityClaims",
                columns: new[] { "IdentityResourceId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IDRES_NAME",
                table: "HclCs_IdentityResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NOTI_TYPE",
                table: "HclCs_Notification",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_Notification_UserId",
                table: "HclCs_Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_PasswordHistory_UserID",
                table: "HclCs_PasswordHistory",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_RoleClaims_RoleId",
                table: "HclCs_RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "HclCs_Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SEC_QUESTION",
                table: "HclCs_SecurityQuestions",
                column: "Question",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_UserClaims_UserId",
                table: "HclCs_UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_UserLogins_UserId",
                table: "HclCs_UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_UserRoles_RoleId",
                table: "HclCs_UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_HclCs_UserRoles_UserId",
                table: "HclCs_UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "HclCs_Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "HclCs_Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USRSEC_QUEID",
                table: "HclCs_UserSecurityQuestions",
                column: "SecurityQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_USRSEC_UID_QUEID",
                table: "HclCs_UserSecurityQuestions",
                columns: new[] { "UserId", "SecurityQuestionId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HclCs_ApiResourceClaims");

            migrationBuilder.DropTable(
                name: "HclCs_ApiScopeClaims");

            migrationBuilder.DropTable(
                name: "HclCs_AuditTrail");

            migrationBuilder.DropTable(
                name: "HclCs_ClientPostLogoutRedirectUris");

            migrationBuilder.DropTable(
                name: "HclCs_ClientRedirectUris");

            migrationBuilder.DropTable(
                name: "HclCs_IdentityClaims");

            migrationBuilder.DropTable(
                name: "HclCs_Notification");

            migrationBuilder.DropTable(
                name: "HclCs_PasswordHistory");

            migrationBuilder.DropTable(
                name: "HclCs_RoleClaims");

            migrationBuilder.DropTable(
                name: "HclCs_SecurityTokens");

            migrationBuilder.DropTable(
                name: "HclCs_UserClaims");

            migrationBuilder.DropTable(
                name: "HclCs_UserLogins");

            migrationBuilder.DropTable(
                name: "HclCs_UserRoles");

            migrationBuilder.DropTable(
                name: "HclCs_UserSecurityQuestions");

            migrationBuilder.DropTable(
                name: "HclCs_UserTokens");

            migrationBuilder.DropTable(
                name: "HclCs_ApiScopes");

            migrationBuilder.DropTable(
                name: "HclCs_Clients");

            migrationBuilder.DropTable(
                name: "HclCs_IdentityResources");

            migrationBuilder.DropTable(
                name: "HclCs_Roles");

            migrationBuilder.DropTable(
                name: "HclCs_SecurityQuestions");

            migrationBuilder.DropTable(
                name: "HclCs_Users");

            migrationBuilder.DropTable(
                name: "HclCs_ApiResources");
        }
    }
}


