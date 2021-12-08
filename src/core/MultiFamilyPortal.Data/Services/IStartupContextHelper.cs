using Microsoft.AspNetCore.Identity;
using MultiFamilyPortal.SaaS.Models;

namespace MultiFamilyPortal.Data.Services
{
    public interface IStartupContextHelper
    {
        Task RunDatabaseAction(Func<MFPContext, Tenant, Task> action);
        Task RunRoleManagerAction(Func<RoleManager<IdentityRole>, Tenant, Task> action);
        Task RunUserManagerAction(Func<UserManager<MFPContext>, Tenant, Task> action);
    }
}
