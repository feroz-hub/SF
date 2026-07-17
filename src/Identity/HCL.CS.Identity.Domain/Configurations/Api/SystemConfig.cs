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
    public string LdapHostName { get; set; }

    public string LdapDomainName { get; set; }

    public int LdapPort { get; set; }

    public bool IsSecureConnection { get; set; }

    public bool IsTwoFactorAuthenticationRequired { get; set; } = false;

    public virtual TwoFactorType TwoFactorType { get; set; } = TwoFactorType.None;
}

public class CryptoConfig
{
    public int RandomStringLength { get; set; } = 32;
}
