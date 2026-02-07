using Microsoft.Extensions.Configuration;
using Starbender.FPrimeSharp.Gds.Topology;

namespace Starbender.FPrimeSharp.Gds.Extensions;

public static class TopologyExtensions
{
    public static string TopologyConfigurationKey = "FPrime:Topology";

    public static TopologyDictionary? GetTopology(this IConfiguration config, string? path = null)
    {
        path ??= TopologyConfigurationKey;

        if (config[TopologyConfigurationKey] == null)
        {
            var topology = new TopologyDictionary();
            config.Bind(path, topology);
            return topology;
        }

        return null;
    }
}
