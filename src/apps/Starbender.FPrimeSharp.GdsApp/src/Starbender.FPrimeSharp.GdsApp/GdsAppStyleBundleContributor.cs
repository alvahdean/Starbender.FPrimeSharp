using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Starbender.FPrimeSharp.GdsApp;

public class GdsAppStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}
