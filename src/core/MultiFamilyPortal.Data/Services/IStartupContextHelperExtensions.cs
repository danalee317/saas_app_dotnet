using Microsoft.AspNetCore.Identity;

namespace MultiFamilyPortal.Data.Services
{
    public static class IStartupContextHelperExtensions
    {
        public static Task RunDatabaseAction(this IStartupContextHelper contextHelper, Func<MFPContext, Task> action) =>
            contextHelper.RunDatabaseAction((db, tenant) => action(db));

        public static Task RunRoleManagerAction(this IStartupContextHelper contextHelper, Func<RoleManager<IdentityRole>, Task> action) =>
            contextHelper.RunRoleManagerAction((roleManager, tenant) => action(roleManager));

        public static Task RunUserManagerAction(this IStartupContextHelper contextHelper, Func<UserManager<MFPContext>, Task> action) =>
            contextHelper.RunUserManagerAction((userManager, tenant) => action(userManager));
    }
}
