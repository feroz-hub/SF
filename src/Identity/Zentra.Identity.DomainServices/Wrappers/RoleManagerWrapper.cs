using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Zentra.DomainServices.Wrappers;

public class RoleManagerWrapper<TRole> : RoleManager<TRole>
    where TRole : class
{
    public RoleManagerWrapper(
        IRoleStore<TRole> store,
        IEnumerable<IRoleValidator<TRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<RoleManagerWrapper<TRole>> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }
}
