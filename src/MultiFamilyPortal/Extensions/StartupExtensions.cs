﻿using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Services;

namespace MultiFamilyPortal.Extensions
{
    public static class StartupExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            Startup.ConfigureServices(builder.Services, builder.Configuration, builder.Environment);
            return builder.Build();
        }

        public static WebApplication ConfigureApplication(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            Startup.ConfigureApp(app, app.Environment);
            return app;
        }

        public static async Task StartAndRunAsync(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                await RunMigrations(app, scope.ServiceProvider);

                var startupTasks = scope.ServiceProvider.GetServices<IStartupTask>();
                foreach (var task in startupTasks)
                {
                    try
                    {
                        await task.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger(nameof(StartupExtensions));
                        logger.LogError(ex, $"Error occurred while attempting to run Startup Task: {task.GetType().FullName}");
                    }
                }
            }

            await app.RunAsync();
        }

        private static async Task RunMigrations(WebApplication app, IServiceProvider services)
        {
            var contextHelper = services.GetRequiredService<IStartupContextHelper>();
            await contextHelper.RunDatabaseAction(async (db, tenant) =>
            {
                try
                {
                    var migrations = await db.Database.GetPendingMigrationsAsync();
                    if (migrations.Any())
                        await db.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(nameof(StartupExtensions));
                    logger.LogError(ex, $"Unable to apply database migrations for Tenant {tenant.Host}");
                }
            });
        }
    }
}
