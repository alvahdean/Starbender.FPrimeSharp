using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Starbender.FPrimeSharp.Gds;
using Starbender.FPrimeSharp.Gds.Blazor;
using Starbender.FPrimeSharp.GdsApp.Menus;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.WebAssembly.BasicTheme;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.Blazor.WebAssembly;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Blazor.WebAssembly;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Blazor.WebAssembly;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Blazor.WebAssembly;
using Volo.Abp.UI.Navigation;

namespace Starbender.FPrimeSharp.GdsApp;

[DependsOn(
    typeof(GdsAppContractsModule),

    // ABP Framework packages
    typeof(AbpAutofacWebAssemblyModule),

    // Account module packages
    typeof(AbpAccountHttpApiClientModule),

    // OpenIddict module packages
    typeof(AbpOpenIddictDomainSharedModule),

    // Identity module packages
    typeof(AbpIdentityBlazorWebAssemblyModule),
    typeof(AbpIdentityHttpApiClientModule),

    // Permission Management module packages
    typeof(AbpPermissionManagementBlazorWebAssemblyModule),
    typeof(AbpPermissionManagementHttpApiClientModule),

    // Feature Management module packages
    typeof(AbpFeatureManagementBlazorWebAssemblyModule),
    typeof(AbpFeatureManagementHttpApiClientModule),

    // Setting Management module packages
    typeof(AbpSettingManagementHttpApiClientModule),
    typeof(AbpSettingManagementBlazorWebAssemblyModule),

    // Theme
    typeof(AbpAspNetCoreComponentsWebAssemblyBasicThemeModule),

    // GDS Module
    // Setting Management module packages
    typeof(GdsHttpApiClientModule),
    typeof(GdsBlazorModule)
)]
public class GdsAppClientModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpAspNetCoreComponentsWebOptions>(options =>
        {
            options.IsBlazorWebApp = true;
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
        var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

        ConfigureAuthentication(builder);
        ConfigureHttpClient(context, environment);
        ConfigureBlazorise(context);
        ConfigureMudBlazor(context);
        ConfigureRouter(context);
        ConfigureMenu(context);

        context.Services.AddHttpClientProxies(typeof(GdsAppContractsModule).Assembly);
    }

    private void ConfigureRouter(ServiceConfigurationContext _)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(GdsAppClientModule).Assembly;
        });
    }

    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new GdsAppMenuContributor(context.Services.GetConfiguration()));
        });
    }

    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        var licenseKey = context.Configuration["Blazorise:LicenseKey"];

        context.Services
            .AddBlazorise(options =>
            {
                options.ProductToken = licenseKey;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }

    private void ConfigureMudBlazor(ServiceConfigurationContext context)
    {
        context.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        });
    }

    private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddBlazorWebAppServices();
    }

    private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
    {
        context.Services.AddTransient(sp => new HttpClient
        {
            BaseAddress = new Uri(environment.BaseAddress)
        });
    }
}
