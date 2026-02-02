using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class BridgeInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BridgeInstallerModule>();
        });
    }
}
