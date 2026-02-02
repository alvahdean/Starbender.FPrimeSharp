using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Gds;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class GdsApplicationTestBase<TStartupModule> : GdsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
