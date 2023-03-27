using Coupon.API.Infrastructure;
using Microsoft.AspNetCore;
using Serilog.Events;

namespace Coupon.API
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .SeedDatabaseStrategy<CouponContext>(context => new CouponSeed().SeedAsync(context).Wait())
                .SubscribersIntegrationEvents()
                .Run();

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    var config = builder.Build();

                    if (config.GetValue("UseVault", false))
                    {
                        builder.AddAzureKeyVault($"https://{config["Vault:Name"]}.vault.azure.net/", config["Vault:ClientId"], config["Vault:ClientSecret"]);
                    }
                })
                .UseStartup<Startup>()
                .UseSerilog((host, builder) =>
                {
                    builder.MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.WithProperty("ApplicationContext", host.HostingEnvironment.ApplicationName)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Seq(string.IsNullOrWhiteSpace(host.Configuration["Serilog:SeqServerUrl"]) ? "http://seq" : host.Configuration["Serilog:SeqServerUrl"])
                        .WriteTo.Http(string.IsNullOrWhiteSpace(host.Configuration["Serilog:LogstashUrl"]) ? "http://logstash:8080" : host.Configuration["Serilog:LogstashUrl"])
                        .ReadFrom.Configuration(host.Configuration);
                });
    }
}
