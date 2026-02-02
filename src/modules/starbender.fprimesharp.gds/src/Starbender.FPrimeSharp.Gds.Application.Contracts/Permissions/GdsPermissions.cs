using Volo.Abp.Reflection;

namespace Starbender.FPrimeSharp.Gds.Permissions;

public class GdsPermissions
{
    public const string GroupName = "Gds";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(GdsPermissions));
    }
}
