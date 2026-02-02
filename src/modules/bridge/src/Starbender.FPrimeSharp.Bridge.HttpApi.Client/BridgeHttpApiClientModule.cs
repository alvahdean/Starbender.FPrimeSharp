using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FPrimeSharp.Bridge;

[DependsOn(
    typeof(BridgeApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class BridgeHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(BridgeApplicationContractsModule).Assembly,
            BridgeRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BridgeHttpApiClientModule>();
        });

    }
}
