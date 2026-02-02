using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(BridgeApplicationModule),
    typeof(BridgeDomainTestModule)
    )]
public class BridgeApplicationTestModule : AbpModule
{

}
