using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Wrappers;

public class UserStoreWrapper :
    UserStore<Users, Roles, ApplicationDbContext, Guid, UserClaims, UserRoles, UserLogins, UserTokens, RoleClaims>
{
    public UserStoreWrapper(ApplicationDbContext context) : base(context)
    {
        AutoSaveChanges = false;
    }
}
