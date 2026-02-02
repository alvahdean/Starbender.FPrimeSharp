using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(GdsDomainSharedModule)
)]
public class GdsDomainModule : AbpModule
{

}
