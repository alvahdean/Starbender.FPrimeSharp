using Starbender.FPrimeSharp.Gds.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Starbender.FPrimeSharp.Gds;

public abstract class GdsController : AbpControllerBase
{
    protected GdsController()
    {
        LocalizationResource = typeof(GdsResource);
    }
}
