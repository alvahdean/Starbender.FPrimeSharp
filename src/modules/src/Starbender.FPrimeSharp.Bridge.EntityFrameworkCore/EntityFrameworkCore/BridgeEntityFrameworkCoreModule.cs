using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Bridge.EntityFrameworkCore;

[DependsOn(
    typeof(BridgeDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class BridgeEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<BridgeDbContext>(options =>
        {
            options.AddDefaultRepositories<IBridgeDbContext>(includeAllEntities: true);
            
            /* Add custom repositories here. Example:
            * options.AddRepository<Question, EfCoreQuestionRepository>();
            */
        });
    }
}
