#nullable enable
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed record ContainerDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    // Container ID
    [JsonPropertyName("id")]
    public required uint Id { get; init; }

    // Default priority for records emitted into this container
    [JsonPropertyName("defaultPriority")]
    public required int DefaultPriority { get; init; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }
}
