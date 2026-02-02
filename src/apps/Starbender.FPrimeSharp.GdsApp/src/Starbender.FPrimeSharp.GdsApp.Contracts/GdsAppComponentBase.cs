using Starbender.FPrimeSharp.GdsApp.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Starbender.FPrimeSharp.GdsApp;

public abstract class GdsAppComponentBase : AbpComponentBase
{
    protected GdsAppComponentBase()
    {
        LocalizationResource = typeof(GdsAppResource);
    }
}
