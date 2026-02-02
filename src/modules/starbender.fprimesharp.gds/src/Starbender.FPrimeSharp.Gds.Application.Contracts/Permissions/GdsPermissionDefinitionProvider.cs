using Starbender.FPrimeSharp.Gds.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Starbender.FPrimeSharp.Gds.Permissions;

public class GdsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(GdsPermissions.GroupName, L("Permission:Gds"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<GdsResource>(name);
    }
}
