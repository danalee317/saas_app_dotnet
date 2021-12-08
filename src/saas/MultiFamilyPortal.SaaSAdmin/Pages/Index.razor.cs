using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.SaaSAdmin.Pages
{
    public partial class Index
    {
        [Inject]
        private TenantContext _context { get; set; } = default!;

        [Inject]
        private IWebHostEnvironment _hostEnvironment { get; set; } = default!;

        private ObservableCollection<Tenant> _tenants { get; } = new();

        private Tenant? newTenant;
        private TelerikNotification? notification;

        private IEnumerable<string> _environments = new[]
        {
            "Development",
            "Staging",
            "Production"
        };

        protected override async Task OnInitializedAsync()
        {
            await UpdateTenants();
        }

        private async Task UpdateTenants()
        {
            var tenants = await _context.Tenants
                .AsNoTracking()
                .ToArrayAsync();
            _tenants.Clear();
            foreach (var tenant in tenants)
                _tenants.Add(tenant);
        }

        private void OnAddTenantClicked()
        {
            newTenant = new Tenant
            {
                Environment = _hostEnvironment.EnvironmentName
            };
        }

        private void CloseNewTenant()
        {
            newTenant = null;
        }

        private async Task SaveTenant()
        {
            if (newTenant is null || notification is null)
                return;

            if (string.IsNullOrEmpty(newTenant.Host))
            {
                Warn("You must enter a host name");
                return;
            }
            else if(string.IsNullOrEmpty(newTenant.DatabaseName))
            {
                Warn("You must enter a database name");
                return;
            }
            else if(string.IsNullOrEmpty(newTenant.Environment))
            {
                Warn("You must enter an environment name");
                return;
            }
            else if(await _context.Tenants.AnyAsync(x => x.Host == newTenant.Host))
            {
                Warn($"The host name {newTenant.Host} already exists");
                return;
            }

            newTenant.Created = DateTimeOffset.Now;

            await _context.Tenants.AddAsync(newTenant);
            await _context.SaveChangesAsync();

            newTenant = null;
            await UpdateTenants();
        }

        private async Task OnUpdateTenant(GridCommandEventArgs args)
        {
            var tenant = args.Item as Tenant;
            if (tenant == null)
                return;

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();
            await UpdateTenants();
        }

        private async Task OnDeleteTenant(GridCommandEventArgs args)
        {
            var tenant = args.Item as Tenant;
            if (tenant == null)
                return;

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            await UpdateTenants();
        }

        private void Warn(string message)
        {
            notification?.Show(new NotificationModel
            {
                Text = message,
                ThemeColor = ThemeColors.Warning
            });
        }
    }
}
