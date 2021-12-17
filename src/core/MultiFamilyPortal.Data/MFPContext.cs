using System.Reflection;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiFamilyPortal.Data.ModelConfiguration;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.SaaS;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;

namespace MultiFamilyPortal.Data
{
    public class MFPContext : IdentityDbContext<SiteUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IMFPContext, IBlogContext, IPersistedGrantDbContext, IMultiTenantDbContext
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;
        private readonly ITenantProvider _tenantProvider;
        private readonly DatabaseSettings _databaseSettings;

        public MFPContext(DbContextOptions<MFPContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions, ITenantProvider tenantProvider, DatabaseSettings settings)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
            _tenantProvider = tenantProvider;
            _databaseSettings = settings;
        }

        public int TenantId => _tenantProvider.GetTenant()?.Id ?? 0;

        #region Blog
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<PostView> PostViews { get; set; } = default!;
        public DbSet<Subscriber> Subscribers { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        #endregion

        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<SiteTheme> SiteThemes { get; set; }
        public DbSet<Setting> Settings { get; set; } = default!;
        public DbSet<EmailTemplate> EmailTemplates { get; set; } = default!;
        public DbSet<EmailPartialTemplate> EmailPartialTemplates { get; set; } = default!;

        public DbSet<HighlightedUser> HighlightedUsers { get; set; } = default!;
        public DbSet<SocialLink> SocialLinks { get; set; } = default!;
        public DbSet<SocialProvider> SocialProviders { get; set; } = default!;

        public DbSet<CustomContent> CustomContent { get; set; } = default!;

        #region Assets
        public DbSet<AssetParner> AssetParners { get; set; } = default!;
        public DbSet<AssetUnderManagement> AssetsUnderManagement { get; set; } = default!;
        #endregion

        #region Underwriting
        public DbSet<UnderwriterGoal> UnderwriterGoals { get; set; } = default!;
        public DbSet<UnderwritingProspectProperty> UnderwritingPropertyProspects { get; set; } = default!;
        public DbSet<UnderwritingProspectPropertyCapitalImprovements> UnderwritingProspectPropertyCapitalImprovements { get; set; } = default!;
        public DbSet<UnderwritingLineItem> UnderwritingLineItems { get; set; } = default!;
        public DbSet<UnderwritingMortgage> UnderwritingMortgages { get; set; } = default!;
        public DbSet<UnderwritingNote> UnderwritingNotes { get; set; } = default!;
        public DbSet<UnderwritingGuidance> UnderwritingGuidance { get; set; } = default!;
        public DbSet<UnderwritingPropertyUnitModel> UnderwritingPropertyUnitModels { get; set; } = default!;
        public DbSet<UnderwritingPropertyUnit> UnderwritingPropertyUnits { get; set; } = default!;
        public DbSet<UnderwritingPropertyUnitLedger> UnderwritingPropertyUnitsLedger { get; set; } = default!;
        public DbSet<UnderwritingProspectFile> UnderwritingProspectFiles { get; set; } = default!;
        public DbSet<UnderwritingProspectPropertyBucketList> UnderwritingProspectPropertyBucketLists { get; set; } = default!;
        #endregion

        #region Investors
        public DbSet<InvestorProspect> InvestorProspects { get; set; } = default!;
        public DbSet<InvestorTestimonial> InvestorTestimonials { get; set; } = default!;
        public DbSet<EmailPreferences> EmailPreferences { get; set; } = default!;
        #endregion


        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
        public DbSet<Key> Keys { get; set; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => SaveChangesAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this.ConfigureForMultiTenant(optionsBuilder, _databaseSettings, _tenantProvider.GetTenant());
        }

        private StoreOptions GetStoreOptions()
        {
            var type = typeof(MFPContext);
            while(type != null)
            {
                if (!(type.Name.Contains("IdentityUserContext") && type.GenericTypeArguments.Length == 5))
                {
                    type = type.BaseType;
                    continue;
                }

                var method = type.GetMethod("GetStoreOptions", BindingFlags.Instance | BindingFlags.NonPublic);
                return (StoreOptions)method.Invoke(this, Array.Empty<object>());
            }

            return null;
        }

        //private T GetProperty<T>(string name, object instance = null) =>
        //    (T)GetProperty(name, instance);

        //private object GetProperty(string name, object instance = null)
        //{
        //    if (instance is null)
        //        instance = this;

        //    var type = instance.GetType();

        //    while(type != null)
        //    {
        //        var property = type.GetRuntimeProperties().FirstOrDefault(x => x.Name == name);
        //        if(property is null)
        //        {
        //            type = type.BaseType;
        //            continue;
        //        }

        //        return property.GetValue(instance);
        //    }

        //    return default;
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //this.GetService<IPersonalDataProtector>();
            base.OnModelCreating(builder);

            //var dbContextServices = GetProperty("ContextServices");
            //var serviceProvider = GetProperty<IServiceProvider>("InternalServiceProvider", dbContextServices);
            //var resolvedServices = GetProperty<IDictionary>("ResolvedServices", serviceProvider);
            //var generic = resolvedServices.Cast<KeyValuePair<object, object>>();
            //var dataProtector = generic.Select(x => x.Value).FirstOrDefault(x => typeof(IPersonalDataProtector).IsAssignableFrom(x.GetType()));
            //var baseContext = (IdentityUserContext<SiteUser, string, IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityUserToken<string>>)this;
            //    var pdp = baseContext.GetService<IPersonalDataProtector>();
            builder.ConfigureIdentityModels(GetStoreOptions());
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
            builder.ConfigureBlogModels();
            builder.ConfigureSocialProviders();
            builder.ConfigureEmailTemplates();
            builder.ConfigureUnderwriting();

            builder.Entity<AssetParner>()
                .HasKey(x => new { x.PartnerId, x.AssetId });

            builder.Entity<HighlightedUser>()
                .HasKey(x => x.Order);
            builder.HasField<ActivityLog, DateTimeOffset>(x => x.Timestamp);
            builder.HasField<InvestorProspect, DateTimeOffset>(x => x.Timestamp);
        }
    }
}
