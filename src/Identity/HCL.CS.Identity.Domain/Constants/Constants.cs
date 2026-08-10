/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants;

public class Constants
{
    public const string DefaultProvider = "Default";
    public const string DefaultEmailProvider = "Email";
    public const string DefaultPhoneProvider = "Phone";

    public const string DefaultAuthenticatorProvider = "Authenticator";

    // public const string DefaultDbSchema = "hcl-cs";
    public const string CreatedBy = "CreatedBy";

    public const string RoleTable = "Roles";
    public const string UserTable = "Users";

    public const int ColumnLength255 = 255;
    public const int ColumnLength2048 = 2048;

    public static readonly List<string> AllowedAuditTables = new()
    {
        "Roles",
        "RoleClaims",
        "Users",
        "UserClaims",
        "UserRoles",
        "UserSecurityQuestions"
    };

    public static readonly List<string> IgnoredAuditColumns = new()
    {
        "Id",
        "NormalizedName",
        "ConcurrencyStamp",
        "NormalizedUserName",
        "NormalizedEmail",
        "PasswordHash",
        "SecurityStamp",
        "DateOfBirth",
        "RowVersion"
    };

    // public const int CryptographyDerivedKeyLength = 32;
    // public const int CryptographyScryptCost = 262144;
    // public const int CryptographyScryptBlocksize = 8;
    // public const int CryptographyScryptParallel = 1;
    // public const int CryptographyPasswordBcryptCost = 13;
    // public const string CryptographySaltPreformat = "$2a$";
    // public const string CryptographySaltPreformat2 = "$";

    // public const string DatabaseConnectionStringMissing = "Database connection string not configured";
    // public const string EmailAlreadyInUse = "EmailAlreadyInUse";
    // public const string InvalidCredentials = "InvalidCredentials";
    // public const string AuditErrorDbInsert = "Error during inserting to the database";
    // public const string AuditErrorDbGet = "Error Occured when getting data from database";
    // public const string AuditInsertSuccess = "Audit Trail Successfully Inserted";
    // public const string AuditTableErrorCount = "No Audit Records Found";
    // public const string AuditTableUnknownError = "UnKnown Error. Contact Administrator";
    // public const string AuditTableDateRangeError = "From date should not greater than to Date";
    // public const string AuditNullError = "Value cannot be null.";
}

public class ResultCustomConstants
{
    public const string AuthorizeCodeRequestKey = "AuthorizeCodeRequestKey";
    public const string TokenRequestKey = "TokenRequestKey";
}

public class EncryptionKeyConstants
{
    public const string DefaultEncryptionKey = "CSHclCs";
    public const string RequestParameterEncryptedKey = "RPSecurityKey";
    public const string VerificationEncryptedKey = "VerificationSecurityKey";
}

public class LoggerKeyConstants
{
    public const string DefaultLoggerKey = "CSHclCs";
}

public class NotificationConstants
{
    public const string DefaultTemplate = "DefaultTemplate";
    public const string EmailVerification = "EmailVerification";
    public const string PhoneNumberVerification = "PhoneVerification";
    public const string GenerateTwoFactorToken = "GenerateTwoFactorToken";
    public const string EmailVerificationUsingLink = "EmailVerificationUsingLink";
    public const string EmailVerificationUsingToken = "EmailVerificationUsingToken";
    public const string PhoneNumberVerificationToken = "PhoneNumberVerificationToken";
    public const string ResetPasswordUsingToken = "ResetPasswordUsingToken";
}

public class PermissionConstants
{
    public const string Read = ".read";
    public const string Write = ".write";
    public const string Delete = ".delete";
    public const string Manage = ".manage";
}
