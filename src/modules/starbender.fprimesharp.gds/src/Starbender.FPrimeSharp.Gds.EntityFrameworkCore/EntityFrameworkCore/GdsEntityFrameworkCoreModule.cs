using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Gds.EntityFrameworkCore;

[DependsOn(
    typeof(GdsDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class GdsEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<GdsDbContext>(options =>
        {
            options.AddDefaultRepositories<IGdsDbContext>(includeAllEntities: true);
            
            /* Add custom repositories here. Example:
            * options.AddRepository<Question, EfCoreQuestionRepository>();
            */
        });
    }
}
