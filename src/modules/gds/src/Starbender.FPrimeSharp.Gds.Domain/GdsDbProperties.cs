namespace Starbender.FPrimeSharp.Gds;

public static class GdsDbProperties
{
    public static string DbTablePrefix { get; set; } = "Gds";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "Gds";
}
