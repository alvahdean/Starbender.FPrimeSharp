using Starbender.FPrimeSharp.GdsApp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Starbender.FPrimeSharp.GdsApp.Permissions;

public class GdsAppPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(GdsAppPermissions.GroupName);



        //Define your own permissions here. Example:
        //myGroup.AddPermission(GdsAppPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<GdsAppResource>(name);
    }
}
