namespace Starbender.FPrimeSharp.Bridge;

public static class BridgeDbProperties
{
    public static string DbTablePrefix { get; set; } = "Bridge";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "Bridge";
}
