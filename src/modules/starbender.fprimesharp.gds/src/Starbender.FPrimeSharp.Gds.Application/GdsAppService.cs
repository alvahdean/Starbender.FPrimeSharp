using Starbender.FPrimeSharp.Gds.Localization;
using Volo.Abp.Application.Services;

namespace Starbender.FPrimeSharp.Gds;

public abstract class GdsAppService : ApplicationService
{
    protected GdsAppService()
    {
        LocalizationResource = typeof(GdsResource);
        ObjectMapperContext = typeof(GdsApplicationModule);
    }
}
