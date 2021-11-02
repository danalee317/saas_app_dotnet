using MultiFamilyPortal.Data;

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
                var db = scope.ServiceProvider.GetRequiredService<MFPContext>();
                //await db.Database.EnsureDeletedAsync();
                try
                {
                    await db.Database.EnsureCreatedAsync();
                }
                finally
                {
                }

                var startupTasks = scope.ServiceProvider.GetServices<IStartupTask>();
                foreach (var task in startupTasks)
                {
                    await task.StartAsync();
                }
            }

            await app.RunAsync();
        }
    }
}
