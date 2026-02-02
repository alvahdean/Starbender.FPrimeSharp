using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FPrimeSharp.Gds;

[DependsOn(
    typeof(GdsApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class GdsHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(GdsApplicationContractsModule).Assembly,
            GdsRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<GdsHttpApiClientModule>();
        });

    }
}
