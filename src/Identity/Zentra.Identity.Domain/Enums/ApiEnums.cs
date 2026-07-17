namespace Zentra.Domain.Enums;

public enum QueryOption
{
    Date = 1,

    ChangeByAndDate = 2,

    ChangeByAndBetweenDates = 3,

    ChangeBywithActionAndBetweenDates = 4,

    None = 5
}

public enum NotificationStatus
{
    Initiated = 1,

    Delivered = 2,

    Failed = 3,

    Delayed = 4,

    Relayed = 5,

    Expanded = 6,

    queued = 7,

    Sending = 8,

    Sent = 9,

    Undelivered = 10,

    Receiving = 11,

    Received = 12,

    Accepted = 13,

    Scheduled = 14,

    Read = 15,

    Partially = 16
}

public enum NotificationTypes
{
    Email = 1,

    SMS = 2
}

public enum EmailNotificationType
{
    Token = 1,

    Link = 2
}

public enum IdentityProvider
{
    Local = 1,

    Ldap = 2,

    Google = 3
}

public enum AuditType
{
    None = 0,

    Create = 1,

    Update = 2,

    Delete = 3
}

public enum TwoFactorType
{
    None = 0,

    Email = 1,

    Sms = 2,

    AuthenticatorApp = 3
}

public enum SecurityTokenOption
{
    Client = 1,

    User = 2,

    BetweenDates = 3,

    All = 4,

    None = 5
}
