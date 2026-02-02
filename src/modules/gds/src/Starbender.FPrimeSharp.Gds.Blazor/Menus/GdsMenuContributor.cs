using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Starbender.FPrimeSharp.Gds.Blazor.Menus;

public class GdsMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(GdsMenus.Prefix, displayName: "Gds", "/Gds", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
