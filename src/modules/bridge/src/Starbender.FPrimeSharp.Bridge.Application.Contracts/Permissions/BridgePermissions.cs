using Volo.Abp.Reflection;

namespace Starbender.FPrimeSharp.Bridge.Permissions;

public class BridgePermissions
{
    public const string GroupName = "Bridge";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(BridgePermissions));
    }
}
