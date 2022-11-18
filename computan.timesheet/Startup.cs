using Hangfire;
using Hangfire.SqlServer;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace computan.timesheet
{
    public partial class Startup
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["HangfireConnection"].ConnectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    });

            yield return new BackgroundJobServer();
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();
        }
    }
}