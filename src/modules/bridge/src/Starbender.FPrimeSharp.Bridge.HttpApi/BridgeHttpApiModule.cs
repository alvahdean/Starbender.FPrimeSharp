using Localization.Resources.AbpUi;
using Starbender.FPrimeSharp.Bridge.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(BridgeApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class BridgeHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(BridgeHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<BridgeResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
