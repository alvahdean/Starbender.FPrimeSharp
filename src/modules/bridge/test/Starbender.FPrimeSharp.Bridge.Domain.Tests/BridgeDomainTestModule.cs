using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(BridgeDomainModule),
    typeof(BridgeTestBaseModule)
)]
public class BridgeDomainTestModule : AbpModule
{

}
