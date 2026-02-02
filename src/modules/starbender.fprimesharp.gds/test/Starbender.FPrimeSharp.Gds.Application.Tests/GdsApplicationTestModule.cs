using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(GdsApplicationModule),
    typeof(GdsDomainTestModule)
    )]
public class GdsApplicationTestModule : AbpModule
{

}
