using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MultiFamilyPortal.SaaS.Data
{
    internal class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var castedContext = context as IMultiTenantDbContext;
            if (castedContext == null)
            {
                throw new Exception("Unknown DBContext type");
            }

            return new { castedContext.TenantId };
        }
    }
}
