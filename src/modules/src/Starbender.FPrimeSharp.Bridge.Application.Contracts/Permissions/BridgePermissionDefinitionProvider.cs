using Starbender.FPrimeSharp.Bridge.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Starbender.FPrimeSharp.Bridge.Permissions;

public class BridgePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BridgePermissions.GroupName, L("Permission:Bridge"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BridgeResource>(name);
    }
}
