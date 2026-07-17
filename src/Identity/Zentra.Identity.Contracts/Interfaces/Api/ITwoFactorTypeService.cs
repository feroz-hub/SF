using Zentra.Domain;
using Zentra.Domain.Enums;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public partial interface IUserAccountService
{
    Task<FrameworkResult> SetTwoFactorEnabledAsync(Guid userId, bool enabled);

    Task<FrameworkResult> UpdateUserTwoFactorTypeAsync(Guid userId, TwoFactorType twoFactorType);

    Task<IList<string>> GetAllTwoFactorTypeAsync();
}
