using Starbender.FPrimeSharp.Bridge.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Starbender.FPrimeSharp.Bridge;

public abstract class BridgeController : AbpControllerBase
{
    protected BridgeController()
    {
        LocalizationResource = typeof(BridgeResource);
    }
}
