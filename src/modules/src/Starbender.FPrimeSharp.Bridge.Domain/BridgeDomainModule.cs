using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(BridgeDomainSharedModule)
)]
public class BridgeDomainModule : AbpModule
{

}
