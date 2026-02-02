using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Bridge;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class BridgeDomainTestBase<TStartupModule> : BridgeTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
