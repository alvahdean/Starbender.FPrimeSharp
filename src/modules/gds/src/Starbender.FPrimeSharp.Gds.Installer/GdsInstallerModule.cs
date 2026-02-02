using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class GdsInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<GdsInstallerModule>();
        });
    }
}
