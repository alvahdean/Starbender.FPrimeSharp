using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed class StructMemberDefinition
{
    [JsonPropertyName("type")]
    public TypeRef Type { get; set; } = new();

    /// <summary>Member ordering/index in struct layout.</summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Some struct members are “member arrays” and carry a "size" on the member itself (distinct from array type defs).:contentReference[oaicite:8]{index=8}
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; }
}
