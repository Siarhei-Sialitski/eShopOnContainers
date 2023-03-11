using Microsoft.Data.SqlClient;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Extensions;

public static class WebHostExtensions
{
    public static bool IsInKubernetes(this IWebHost host)
    {
        var cfg = host.Services.GetService<IConfiguration>();
        var orchestratorType = cfg.GetValue<string>("OrchestratorType");
        return orchestratorType?.ToUpper() == "K8S";
    }

    public static IWebHost SeedDatabaseStrategy<TContext>(this IWebHost host, Action<TContext> seeder)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<TContext>();

            var policy = Policy.Handle<SqlException>()
                .WaitAndRetry(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(8),
                });

            policy.Execute(() =>
            {
                seeder.Invoke(context);
            });
        }

        return host;
    }
}
