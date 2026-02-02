using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(GdsDomainModule),
    typeof(GdsTestBaseModule)
)]
public class GdsDomainTestModule : AbpModule
{

}
