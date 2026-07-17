using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZentraInstallerMVC.Infrastructure.Persistence.Migrations.Sqlite
{
    public partial class ZentraSqliteV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zentra_ApiResources",
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
                    table.PrimaryKey("PK_Zentra_ApiResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_AuditTrail",
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
                    table.PrimaryKey("PK_Zentra_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_Clients",
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
                    table.PrimaryKey("PK_Zentra_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_IdentityResources",
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
                    table.PrimaryKey("PK_Zentra_IdentityResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_Roles",
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
                    table.PrimaryKey("PK_Zentra_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_SecurityQuestions",
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
                    table.PrimaryKey("PK_Zentra_SecurityQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_SecurityTokens",
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
                    table.PrimaryKey("PK_Zentra_SecurityTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_Users",
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
                    table.PrimaryKey("PK_Zentra_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_ApiResourceClaims",
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
                    table.PrimaryKey("PK_Zentra_ApiResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_ApiResourceClaims_Zentra_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "Zentra_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_ApiScopes",
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
                    table.PrimaryKey("PK_Zentra_ApiScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_ApiScopes_Zentra_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "Zentra_ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_ClientPostLogoutRedirectUris",
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
                    table.PrimaryKey("PK_Zentra_ClientPostLogoutRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_ClientPostLogoutRedirectUris_Zentra_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Zentra_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_ClientRedirectUris",
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
                    table.PrimaryKey("PK_Zentra_ClientRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_ClientRedirectUris_Zentra_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Zentra_Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_IdentityClaims",
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
                    table.PrimaryKey("PK_Zentra_IdentityClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_IdentityClaims_Zentra_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "Zentra_IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_RoleClaims",
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
                    table.PrimaryKey("PK_Zentra_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_RoleClaims_Zentra_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Zentra_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_Notification",
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
                    table.PrimaryKey("PK_Zentra_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_Notification_Zentra_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_PasswordHistory",
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
                    table.PrimaryKey("PK_Zentra_PasswordHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_PasswordHistory_Zentra_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_UserClaims",
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
                    table.PrimaryKey("PK_Zentra_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_UserClaims_Zentra_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_UserLogins",
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
                    table.PrimaryKey("PK_Zentra_UserLogins", x => new { x.LoginProvider, x.ProviderKey, x.UserId });
                    table.ForeignKey(
                        name: "FK_Zentra_UserLogins_Zentra_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_UserRoles",
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
                    table.PrimaryKey("PK_Zentra_UserRoles", x => new { x.Id, x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Zentra_UserRoles_Zentra_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Zentra_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zentra_UserRoles_Zentra_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_UserSecurityQuestions",
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
                    table.PrimaryKey("PK_Zentra_UserSecurityQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_UserSecurityQuestions_Zentra_SecurityQuestions_SecurityQuestionId",
                        column: x => x.SecurityQuestionId,
                        principalTable: "Zentra_SecurityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zentra_UserSecurityQuestions_Zentra_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_UserTokens",
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
                    table.PrimaryKey("PK_Zentra_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_Zentra_UserTokens_Zentra_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Zentra_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zentra_ApiScopeClaims",
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
                    table.PrimaryKey("PK_Zentra_ApiScopeClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zentra_ApiScopeClaims_Zentra_ApiScopes_ApiScopeId",
                        column: x => x.ApiScopeId,
                        principalTable: "Zentra_ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APIRES_CLM_RESID_TYPE",
                table: "Zentra_ApiResourceClaims",
                columns: new[] { "ApiResourceId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APIRES_NAME",
                table: "Zentra_ApiResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APISCO_CLM_SCOID_TYPE",
                table: "Zentra_ApiScopeClaims",
                columns: new[] { "ApiScopeId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APISCO_SCOID_NAME",
                table: "Zentra_ApiScopes",
                columns: new[] { "ApiResourceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUD_CBBY_ACTY",
                table: "Zentra_AuditTrail",
                columns: new[] { "CreatedBy", "ActionType" });

            migrationBuilder.CreateIndex(
                name: "IX_AUD_CRON_ACTY",
                table: "Zentra_AuditTrail",
                columns: new[] { "CreatedOn", "ActionType" });

            migrationBuilder.CreateIndex(
                name: "IX_AUD_CRON_CBBY",
                table: "Zentra_AuditTrail",
                columns: new[] { "CreatedOn", "CreatedBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri",
                table: "Zentra_ClientPostLogoutRedirectUris",
                columns: new[] { "ClientId", "PostLogoutRedirectUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_ClientRedirectUris_ClientId_RedirectUri",
                table: "Zentra_ClientRedirectUris",
                columns: new[] { "ClientId", "RedirectUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLI_CLID_CLSEC",
                table: "Zentra_Clients",
                columns: new[] { "ClientId", "ClientSecret" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IDRESCLM_IDRESID_TYPE",
                table: "Zentra_IdentityClaims",
                columns: new[] { "IdentityResourceId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IDRES_NAME",
                table: "Zentra_IdentityResources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NOTI_TYPE",
                table: "Zentra_Notification",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_Notification_UserId",
                table: "Zentra_Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_PasswordHistory_UserID",
                table: "Zentra_PasswordHistory",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_RoleClaims_RoleId",
                table: "Zentra_RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Zentra_Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SEC_QUESTION",
                table: "Zentra_SecurityQuestions",
                column: "Question",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_UserClaims_UserId",
                table: "Zentra_UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_UserLogins_UserId",
                table: "Zentra_UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_UserRoles_RoleId",
                table: "Zentra_UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Zentra_UserRoles_UserId",
                table: "Zentra_UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Zentra_Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Zentra_Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USRSEC_QUEID",
                table: "Zentra_UserSecurityQuestions",
                column: "SecurityQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_USRSEC_UID_QUEID",
                table: "Zentra_UserSecurityQuestions",
                columns: new[] { "UserId", "SecurityQuestionId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zentra_ApiResourceClaims");

            migrationBuilder.DropTable(
                name: "Zentra_ApiScopeClaims");

            migrationBuilder.DropTable(
                name: "Zentra_AuditTrail");

            migrationBuilder.DropTable(
                name: "Zentra_ClientPostLogoutRedirectUris");

            migrationBuilder.DropTable(
                name: "Zentra_ClientRedirectUris");

            migrationBuilder.DropTable(
                name: "Zentra_IdentityClaims");

            migrationBuilder.DropTable(
                name: "Zentra_Notification");

            migrationBuilder.DropTable(
                name: "Zentra_PasswordHistory");

            migrationBuilder.DropTable(
                name: "Zentra_RoleClaims");

            migrationBuilder.DropTable(
                name: "Zentra_SecurityTokens");

            migrationBuilder.DropTable(
                name: "Zentra_UserClaims");

            migrationBuilder.DropTable(
                name: "Zentra_UserLogins");

            migrationBuilder.DropTable(
                name: "Zentra_UserRoles");

            migrationBuilder.DropTable(
                name: "Zentra_UserSecurityQuestions");

            migrationBuilder.DropTable(
                name: "Zentra_UserTokens");

            migrationBuilder.DropTable(
                name: "Zentra_ApiScopes");

            migrationBuilder.DropTable(
                name: "Zentra_Clients");

            migrationBuilder.DropTable(
                name: "Zentra_IdentityResources");

            migrationBuilder.DropTable(
                name: "Zentra_Roles");

            migrationBuilder.DropTable(
                name: "Zentra_SecurityQuestions");

            migrationBuilder.DropTable(
                name: "Zentra_Users");

            migrationBuilder.DropTable(
                name: "Zentra_ApiResources");
        }
    }
}


