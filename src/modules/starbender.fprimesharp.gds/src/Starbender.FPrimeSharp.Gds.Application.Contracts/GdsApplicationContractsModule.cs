using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(GdsDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class GdsApplicationContractsModule : AbpModule
{

}
