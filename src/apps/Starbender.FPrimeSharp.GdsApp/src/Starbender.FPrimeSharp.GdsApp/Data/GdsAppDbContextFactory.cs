using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Starbender.FPrimeSharp.GdsApp.Data;

public class GdsAppDbContextFactory : IDesignTimeDbContextFactory<GdsAppDbContext>
{
    public GdsAppDbContext CreateDbContext(string[] args)
    {
        GdsAppGlobalFeatureConfigurator.Configure();
        GdsAppModuleExtensionConfigurator.Configure();
        
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<GdsAppDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));

        return new GdsAppDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}