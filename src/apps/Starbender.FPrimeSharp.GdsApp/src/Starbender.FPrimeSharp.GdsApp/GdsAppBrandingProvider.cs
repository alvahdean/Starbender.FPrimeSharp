using Microsoft.Extensions.Localization;
using Starbender.FPrimeSharp.GdsApp.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Starbender.FPrimeSharp.GdsApp;

[Dependency(ReplaceServices = true)]
public class GdsAppBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<GdsAppResource> _localizer;

    public GdsAppBrandingProvider(IStringLocalizer<GdsAppResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
