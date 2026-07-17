using Microsoft.AspNetCore.Identity;

namespace Zentra.Domain.Entities.Api;
// TODO: Jesu needs to remove this class as it is not required

public class UserTokens : IdentityUserToken<Guid>
{
    public virtual bool IsDeleted { get; set; } = false;
}
