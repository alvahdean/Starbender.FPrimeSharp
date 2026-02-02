using Starbender.FPrimeSharp.Gds.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Starbender.FPrimeSharp.Gds.Blazor;

public abstract class GdsComponentBase : AbpComponentBase
{
    protected GdsComponentBase()
    {
        LocalizationResource = typeof(GdsResource);
    }
}