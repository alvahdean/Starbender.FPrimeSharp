using Starbender.FPrimeSharp.Bridge.Localization;
using Volo.Abp.Application.Services;

namespace Starbender.FPrimeSharp.Bridge;

public abstract class BridgeAppService : ApplicationService
{
    protected BridgeAppService()
    {
        LocalizationResource = typeof(BridgeResource);
        ObjectMapperContext = typeof(BridgeApplicationModule);
    }
}
