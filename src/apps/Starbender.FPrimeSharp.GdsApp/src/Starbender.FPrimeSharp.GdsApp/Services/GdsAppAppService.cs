using Volo.Abp.Application.Services;
using Starbender.FPrimeSharp.GdsApp.Localization;

namespace Starbender.FPrimeSharp.GdsApp.Services;

/* Inherit your application services from this class. */
public abstract class GdsAppAppService : ApplicationService
{
    protected GdsAppAppService()
    {
        LocalizationResource = typeof(GdsAppResource);
    }
}