using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HCL.CS.DomainServices.Wrappers;

public class EmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : class
{
    public EmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
}

public class ChangePhoneNumberTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : class
{
    public ChangePhoneNumberTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ChangePhoneNumberTokenProviderOption> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

public class ChangePhoneNumberTokenProviderOption : DataProtectionTokenProviderOptions
{
}

public class PasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : class
{
    public PasswordResetTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<PasswordResetTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
}

public class UserTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : class
{
    public UserTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<UserTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

public class UserTokenProviderOptions : DataProtectionTokenProviderOptions
{
}
