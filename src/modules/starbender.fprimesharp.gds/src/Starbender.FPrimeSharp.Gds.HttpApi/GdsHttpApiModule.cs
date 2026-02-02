using Localization.Resources.AbpUi;
using Starbender.FPrimeSharp.Gds.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(GdsApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class GdsHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(GdsHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<GdsResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
