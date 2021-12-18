using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data.Services;

namespace MultiFamilyPortal.Data.Internals
{
    public class DbContextStartupTask : IStartupTask
    {
        private const string REMentor = "RE Mentor";
        private const string TrainingSupport = "Training & Support";
        private const string SoftwareVendor = "Software Vendor";

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
            await SeedCrmRoles(db);
            await SeedDefaultContacts(db);
        }

        private async Task SeedDefaultContacts(ICRMContext db)
        {
            if(!await db.CrmContacts.AnyAsync())
            {
                foreach(var contact in DefaultCrmContacts)
                {
                    if(!await db.CrmContacts.AnyAsync(x => x.FirstName == contact.FirstName && x.LastName == contact.LastName))
                    {
                        var roleName = contact.Company == "RE Mentor" ? TrainingSupport : SoftwareVendor;
                        var role = await db.CrmContactRoles.FirstOrDefaultAsync(x => x.Name == roleName);
                        contact.Roles = new[] { role };
                        await db.CrmContacts.AddAsync(contact);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task SeedCrmRoles(ICRMContext db)
        {
            foreach(var role in DefaultCrmRoles)
            {
                if(!await db.CrmContactRoles.AnyAsync(x => x.Name == role.Name))
                {
                    await db.CrmContactRoles.AddAsync(role);
                    await db.SaveChangesAsync();
                }
            }
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

        public static readonly IEnumerable<CRMContact> DefaultCrmContacts = new[]
        {
            new CRMContact
            {
                FirstName = "Dan",
                LastName = "Siegel",
                Company = "AvantiPoint",
                Title = "CEO",
                Emails = new []
                {
                    new CRMContactEmail
                    {
                        Email = "dsiegel@avantipoint.com",
                        Primary = true,
                        Type = CRMContactEmailType.Work
                    }
                },
                Logs = new []
                {
                    new CRMContactLog
                    {
                        Notes = "Welcome to the MultiFamily Business Portal. If you run into any trouble or have any questions please reach out any time."
                    }
                }
            },
            new CRMContact
            {
                FirstName = "Jeannie",
                LastName = "Orlowski",
                Company = REMentor,
                Title = "Investor Relations / Coach",
                Emails = new []
                {
                    new CRMContactEmail
                    {
                        Email = "jeannie@rementor.com",
                        Primary = true,
                        Type = CRMContactEmailType.Work
                    }
                },
                Phones = new []
                {
                    new CRMContactPhone
                    {
                        Number = "7819825724",
                        Primary = true,
                        Type = CRMContactPhoneType.Work
                    }
                },
            },
            new CRMContact
            {
                FirstName = "Jermaine",
                LastName = "Evans",
                Company = REMentor,
                Title = "Coaching Coordinator",
                Emails = new []
                {
                    new CRMContactEmail
                    {
                        Email = "jevans@rementor.com",
                        Primary = true,
                        Type = CRMContactEmailType.Work
                    }
                },
                Phones = new []
                {
                    new CRMContactPhone
                    {
                        Number = "7819825711",
                        Primary = true,
                        Type = CRMContactPhoneType.Work
                    }
                }
            },
        };

        private static readonly IEnumerable<CRMContactRole> DefaultCrmRoles = new[]
        {
            new CRMContactRole
            {
                Name = "Real Estate Broker",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Loan Broker",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Direct Lender",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Hard Money Lender",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Investor",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Sponsor",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Mentor",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Insurance Agent",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Property Manager",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Attorney",
                CoreTeam = true,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Property Inspector",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Pool Maintenance",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Lawn Maintenance",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "General Contractor",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "HVAC",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Electrician",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Plumber",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Local Government",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = "Economic Development",
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = TrainingSupport,
                SystemDefined = true,
            },
            new CRMContactRole
            {
                Name = SoftwareVendor,
                SystemDefined = true,
            }
        };

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
