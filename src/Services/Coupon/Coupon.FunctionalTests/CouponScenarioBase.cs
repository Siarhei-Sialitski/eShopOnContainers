using Coupon.API;
using Coupon.API.Infrastructure;
using Coupon.API.Infrastructure.Repositories;
using Microsoft.eShopOnContainers.Services.Catalog.API.Extensions;

namespace Coupon.FunctionalTests;

public class CouponScenariosBase
{
    public TestServer CreateServer()
    {
        var path = Assembly.GetAssembly(typeof(CouponScenariosBase))
            .Location;

        var hostBuilder = new WebHostBuilder()
            .UseContentRoot(Path.GetDirectoryName(path))
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
            })
            
            .UseStartup<Startup>();


        var testServer = new TestServer(hostBuilder);

        testServer.Host
            .SeedDatabaseStrategy<CouponContext>(context => new CouponSeed().SeedAsync(context).Wait());

        return testServer;
    }

    public static class Get
    {
        public static string CouponByCode(string code)
        {
            return $"api/v1/coupon/{code}";
        }
    }
}
