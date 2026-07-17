using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Wrappers;

public class RoleStoreWrapper : RoleStore<Roles, ApplicationDbContext, Guid, UserRoles, RoleClaims>
{
    public RoleStoreWrapper(ApplicationDbContext context) : base(context)
    {
        AutoSaveChanges = false;
    }
}
