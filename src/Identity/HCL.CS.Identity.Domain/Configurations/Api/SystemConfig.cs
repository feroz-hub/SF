/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Configurations.Api;

public class DBConfig
{
    public DbTypes Database { get; set; }

    public string DBConnectionString { get; set; }
}

public class LoginConfig
{
    public bool IsPersistent { get; set; } = false;

    public bool LockoutOnFailure { get; set; } = true;

    public bool RememberClient { get; set; } = false;
}

public class UserConfig
{
    public int MinUserNameLength { get; set; } = 6;

    public int MaxUserNameLength { get; set; } = 255;

    public int MinFirstAndLastNameLength { get; set; } = 2;

    public int MaxFirstAndLastNameLength { get; set; } = 255;

    public int MinPhoneNumberLength { get; set; } = 4;

    public int MaxPhoneNumberLength { get; set; } = 15;

    public int MinNoOfQuestions { get; set; } = 3;

    public int MinSecurityAnswersLength { get; set; } = 3;

    public int MinDOBYear { get; set; } = 18;

    public int MaxDOBYear { get; set; } = 100;

    public int AccessFailedCount { get; set; } = 3;

    public int RequiredRecoveryCodes { get; set; } = 5;

    public bool RequireUniqueEmail { get; set; } = true;

    public bool RequireConfirmedEmail { get; set; } = true;

    public bool RequireConfirmedPhoneNumber { get; set; } = true;

    public bool LockOutAllowedForNewUsers { get; set; } = true;

    public int DefaultLockoutTimeSpanMin { get; set; } = 10;

    public int MaxRetryInvalidCredentials { get; set; } = 3;

    public int EmailTokenExpiry { get; set; } = 720;

    public int OTPTokenExpiry { get; set; } = 10;

    public int PasswordResetTokenExpiry { get; set; } = 10;

    public int UserTokenExpiry { get; set; } = 10;

    public int LockAccountPeriod { get; set; } = 45;

    public string DefaultUserRole { get; set; } = "HclCsUser";

    public int MaxFailedAccessAttempts { get; set; } = 5;
}

public class PasswordConfig
{
    public int MinPasswordLength { get; set; } = 8;

    public int MaxPasswordLength { get; set; } = 64;

    public int RequiredUniqueChars { get; set; } = 8;

    public bool RequireDigit { get; set; } = true;

    public bool RequireLowercase { get; set; } = true;

    public bool RequireUppercase { get; set; } = true;

    public bool RequireSpecialChar { get; set; } = true;

    public string PasswordPattern { get; set; }

    public int MaxLimitPasswordReuse { get; set; } = 10;

    public int MaxPasswordExpiry { get; set; } = 42;

    public int PasswordNotificationBeforeExpiry { get; set; } = 3;
}

public class EmailConfig
{
    public string SmtpServer { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public EmailNotificationType EmailNotificationType { get; set; } = EmailNotificationType.Link;

    public bool SecureSocketOptions { get; set; }
}

public class EmailTemplate
{
    public string Name { get; set; }

    public string Subject { get; set; }

    public string FromAddress { get; set; }

    public string FromName { get; set; }

    public string CC { get; set; }

    public string TemplateFormat { get; set; }
}

public class SMSConfig
{
    public string SMSAccountIdentification { get; set; }

    public string SMSAccountPassword { get; set; }

    public string SMSAccountFrom { get; set; }

    public string SMSStatusCallbackURL { get; set; }
}

public class SMSTemplate
{
    public string Name { get; set; }

    public string TemplateFormat { get; set; }
}

public class LdapConfig
{
    public bool? Enabled { get; set; }

    public string LdapHostName { get; set; }

    public string LdapDomainName { get; set; }

    public int LdapPort { get; set; } = 636;

    public bool IsSecureConnection { get; set; }

    public bool UseSsl { get; set; } = true;

    public bool UseStartTls { get; set; }

    public bool AllowUnencryptedForDevelopment { get; set; }

    public string UserSearchBase { get; set; }

    public string UserSearchFilter { get; set; } = "(userPrincipalName={0})";

    public string BindDn { get; set; }

    public string BindPassword { get; set; }

    public int ConnectTimeoutSeconds { get; set; } = 10;

    public int SearchTimeoutSeconds { get; set; } = 10;

    public bool RequireEmployeeId { get; set; }

    public bool RequireDepartment { get; set; }

    public bool RequireUserPrincipalName { get; set; }

    public LdapAttributeConfig Attributes { get; set; } = new();

    public bool IsTwoFactorAuthenticationRequired { get; set; } = false;

    public virtual TwoFactorType TwoFactorType { get; set; } = TwoFactorType.None;

    public bool IsEnabled =>
        Enabled ?? (!string.IsNullOrWhiteSpace(LdapHostName) && LdapPort > 0);

    public bool IsSslEnabled => UseSsl || IsSecureConnection;
}

public class LdapAttributeConfig
{
    public string ImmutableId { get; set; } = "objectGUID";

    public string EmployeeId { get; set; } = "employeeID";

    public string UserPrincipalName { get; set; } = "userPrincipalName";

    public string Email { get; set; } = "mail";

    public string DisplayName { get; set; } = "displayName";

    public string Department { get; set; } = "department";

    public string AccountStatus { get; set; } = "userAccountControl";
}

public class LocalAuthenticationConfig
{
    public bool EnabledWhenLdapDisabledOrUnconfigured { get; set; } = true;

    public List<string> AllowedEmailDomains { get; set; } = ["hcltech.com"];

    public bool RequireEmailConfirmation { get; set; } = true;

    public bool AllowSelfRegistration { get; set; } = true;

    public bool AllowAdministratorCreation { get; set; } = true;
}

public class CryptoConfig
{
    public int RandomStringLength { get; set; } = 32;
}
