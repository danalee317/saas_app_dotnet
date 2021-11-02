using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Data.Internals
{
    public class DbContextStartupTask : IStartupTask
    {
        private MFPContext _db { get; }

        public DbContextStartupTask(MFPContext db)
        {
            _db = db;
        }

        public async Task StartAsync()
        {
            await SeedSettings();
            await SeedUnderwritingGuidance();
        }

        private async Task SeedUnderwritingGuidance()
        {
            foreach(var guidance in DefaultGuidance)
            {
                if (await _db.UnderwritingGuidance.AnyAsync(x => string.IsNullOrEmpty(x.Market) && x.Category == guidance.Category))
                    continue;

                _db.UnderwritingGuidance.Add(guidance);
                await _db.SaveChangesAsync();
            }
        }

        private async Task SeedSettings()
        {
            var type = typeof(PortalSetting);
            var props = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach(var prop in props)
            {
                if (await _db.Settings.AnyAsync(x => x.Key == prop.Name))
                    continue;

                var defaultValueAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
                _db.Settings.Add(new Models.Setting {
                    Key = prop.Name,
                    Value = !string.IsNullOrEmpty(defaultValueAttr?.Value?.ToString()) ? defaultValueAttr.Value.ToString() : string.Empty,
                });
                await _db.SaveChangesAsync();
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
