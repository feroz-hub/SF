using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Wrappers;

public class RoleStoreWrapper : RoleStore<Roles, ApplicationDbContext, Guid, UserRoles, RoleClaims>
{
    public RoleStoreWrapper(ApplicationDbContext context) : base(context)
    {
        AutoSaveChanges = false;
    }
}
