using Volo.Abp.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Starbender.FPrimeSharp.GdsApp.Data;

public class GdsAppDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public GdsAppDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        
        /* We intentionally resolving the GdsAppDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<GdsAppDbContext>()
            .Database
            .MigrateAsync();

    }
}
