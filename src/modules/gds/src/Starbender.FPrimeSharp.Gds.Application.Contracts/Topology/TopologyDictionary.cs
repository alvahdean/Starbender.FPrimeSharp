#nullable enable
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

/// <summary>
/// In this dictionary a struct member’s type is always a TypeReference.
/// That TypeReference is either a primitive-ish kind(integer, float, bool, string) 
/// or a reference to another named type via qualifiedIdentifier.
/// 
/// Arrays show up in two ways:
/// 1. As a named type definition with kind: "array" (with elementType + size)
/// 2. As a “member array” where the struct member itself has a size property
/// </summary>
public sealed class TopologyDictionary
{
    [JsonPropertyName("metadata")]
    public DictionaryMetadata? Metadata { get; set; }

    [JsonPropertyName("typeDefinitions")]
    public List<TypeDefinition> TypeDefinitions { get; set; } = new();

    [JsonPropertyName("constants")]
    public List<ConstantDefinition> Constants { get; set; } = new();

    [JsonPropertyName("commands")]
    public List<CommandDefinition> Commands { get; set; } = new();

    [JsonPropertyName("parameters")]
    public List<ParameterDefinition> Parameters { get; set; } = new();

    [JsonPropertyName("events")]
    public List<EventDefinition> Events { get; set; } = new();

    [JsonPropertyName("telemetryChannels")]
    public List<TelemetryChannelDefinition> TelemetryChannels { get; set; } = new();

    [JsonPropertyName("records")]
    public List<RecordDefinition> Records { get; set; } = new();

    [JsonPropertyName("containers")]
    public List<ContainerDefinition> Containers { get; set; } = new();

    [JsonPropertyName("telemetryPacketSets")]
    public List<TelemetryPacketSetDefinition> TelemetryPacketSets { get; set; } = new();
}

