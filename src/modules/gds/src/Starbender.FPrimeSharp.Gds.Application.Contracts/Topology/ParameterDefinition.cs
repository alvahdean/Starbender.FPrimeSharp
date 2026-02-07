#nullable enable

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed record ParameterDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("type")]
    public required TypeRef Type { get; init; }

    /// <summary>
    /// Parameter identifier. In this dictionary it aligns with the *_PRM_SET opcode.
    /// </summary>
    [JsonPropertyName("id")]
    public required uint Id { get; init; }

    /// <summary>
    /// Optional default value. The JSON value shape depends on <see cref="Type"/>.
    /// </summary>
    [JsonPropertyName("default")]
    public JsonElement? Default { get; init; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }

    /// <summary>
    /// Derived convenience: the generated "set" command opcode commonly equals Id.
    /// </summary>
    [JsonIgnore]
    public uint SetOpcode => Id;

    /// <summary>
    /// Derived convenience: the generated "save" command opcode commonly equals Id + 1.
    /// </summary>
    [JsonIgnore]
    public uint SaveOpcode => checked(Id + 1);
}
