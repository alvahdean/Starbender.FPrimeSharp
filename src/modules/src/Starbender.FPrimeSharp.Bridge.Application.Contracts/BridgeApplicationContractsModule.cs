using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(BridgeDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class BridgeApplicationContractsModule : AbpModule
{

}
