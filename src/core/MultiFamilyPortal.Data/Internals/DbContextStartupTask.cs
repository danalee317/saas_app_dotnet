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
        private DatabaseContextSeeder _databaseContextSeeder { get; }

        public DbContextStartupTask(IStartupContextHelper contextHelper, DatabaseContextSeeder seeder)
        {
            _contextHelper = contextHelper;
            _databaseContextSeeder = seeder;
        }

        public async Task StartAsync()
        {
            await _contextHelper.RunDatabaseAction(_databaseContextSeeder.SeedAsync);
        }
    }
}
