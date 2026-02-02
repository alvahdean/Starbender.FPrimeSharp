using Volo.Abp.Modularity;

namespace Starbender.FPrimeSharp.Bridge;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class BridgeApplicationTestBase<TStartupModule> : BridgeTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
