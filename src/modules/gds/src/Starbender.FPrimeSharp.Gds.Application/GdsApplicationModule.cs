using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Starbender.FPrimeSharp.Gds.Extensions;
using Starbender.FPrimeSharp.Gds.Topology;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(GdsDomainModule),
    typeof(GdsApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class GdsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<GdsApplicationModule>();
    }
}
