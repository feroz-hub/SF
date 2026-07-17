using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Wrappers;

public class UserStoreWrapper :
    UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins, UserTokens, RoleClaims>
{
    public UserStoreWrapper(ApplicationDbContext context) : base(context)
    {
        AutoSaveChanges = false;
    }
}
