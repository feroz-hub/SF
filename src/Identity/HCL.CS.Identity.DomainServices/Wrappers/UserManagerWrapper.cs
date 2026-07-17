using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.DomainServices.Wrappers;

public class UserManagerWrapper<TUser> : UserManager<TUser>
    where TUser : class
{
    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private readonly IUserTokenRepository userTokenRepository;

    public UserManagerWrapper(
        IUserStore<TUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManagerWrapper<TUser>> logger,
        IUserTokenRepository userTokenRepository)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
            services, logger)
    {
        this.userTokenRepository = userTokenRepository;
    }

    public async Task<SetupCode> GenerateAuthenticatorSetupCode(string applicationName, string email, string key)
    {
        var twoFactorAuthenticator = new TwoFactorAuthenticator();
        var setupCode = twoFactorAuthenticator.GenerateSetupCode(applicationName, email, key, false);
        return await Task.FromResult(setupCode);
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(Guid userId, string token)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        var entry = await GetUserToken(userId, InternalLoginProvider, AuthenticatorKeyTokenName);
        if (entry != null)
        {
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            return twoFactorAuthenticator.ValidateTwoFactorPIN(entry.Value, token);
        }

        return false;
    }

    private async Task<UserTokens?> GetUserToken(Guid userId, string loginProvider, string name)
    {
        var userToken = await userTokenRepository.GetUserTokenAsync(userId, name, loginProvider);
        if (userToken != null && userToken.Count > 0) return userToken[0];

        return null;
    }
}
