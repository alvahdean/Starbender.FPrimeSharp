using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Gds;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class GdsDomainTestBase<TStartupModule> : GdsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
