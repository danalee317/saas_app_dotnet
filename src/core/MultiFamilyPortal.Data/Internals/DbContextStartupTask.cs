using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data.Services;

namespace MultiFamilyPortal.Data.Internals
{
    public class DbContextStartupTask : IStartupTask
    {
        private IStartupContextHelper _contextHelper { get; }

        public DbContextStartupTask(IStartupContextHelper contextHelper)
        {
            _contextHelper = contextHelper;
        }

        public async Task StartAsync()
        {
            await _contextHelper.RunDatabaseAction(SeedInternal);
        }

        private async Task SeedInternal(MFPContext db)
        {
            await SeedSettings(db);
            await SeedUnderwritingGuidance(db);
        }

        private async Task SeedUnderwritingGuidance(MFPContext db)
        {
            foreach(var guidance in DefaultGuidance)
            {
                if (await db.UnderwritingGuidance.AnyAsync(x => string.IsNullOrEmpty(x.Market) && x.Category == guidance.Category))
                    continue;

                db.UnderwritingGuidance.Add(guidance);
                await db.SaveChangesAsync();
            }
        }

        private async Task SeedSettings(MFPContext db)
        {
            var type = typeof(PortalSetting);
            var props = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach(var prop in props)
            {
                if (await db.Settings.AnyAsync(x => x.Key == prop.Name))
                    continue;

                var defaultValueAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
                db.Settings.Add(new Models.Setting {
                    Key = prop.Name,
                    Value = !string.IsNullOrEmpty(defaultValueAttr?.Value?.ToString()) ? defaultValueAttr.Value.ToString() : string.Empty,
                });
                await db.SaveChangesAsync();
            }
        }

        private static readonly IEnumerable<UnderwritingGuidance> DefaultGuidance = new[]
        {
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.Taxes,
                Min = 0,
                Max = 0.01,
                Type = CostType.PercentOfPurchase,
                IgnoreOutOfRange = true,
            },
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.Insurance,
                Min = 250,
                Max = 250,
                Type = CostType.PerDoor
            },
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.RepairsMaintenance,
                Min = 300,
                Max = 600,
                Type = CostType.PerDoor
            },
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.GeneralAdmin,
                Min = 100,
                Max = 250,
                Type = CostType.PerDoor
            },
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.Marketing,
                Min = 100,
                Max = 100,
                Type = CostType.PerDoor
            },
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.ContractServices,
                Min = 200,
                Max = 400,
                Type = CostType.PerDoor
            },
            new UnderwritingGuidance
            {
                Category = UnderwritingCategory.Payroll,
                Min = 700,
                Max = 1000,
                Type = CostType.PerDoor
            },
        };
    }
}
