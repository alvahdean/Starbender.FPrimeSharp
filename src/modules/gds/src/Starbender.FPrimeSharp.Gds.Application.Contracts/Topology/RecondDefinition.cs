using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed class RecordDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public TypeRef Type { get; set; } = new();

    [JsonPropertyName("array")]
    public bool Array { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; }
}
