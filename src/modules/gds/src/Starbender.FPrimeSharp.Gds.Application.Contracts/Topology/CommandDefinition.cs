#nullable enable

using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed record CommandDefinition
{
    /// <summary>
    /// Fully qualified command name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Command opcode (unique within a deployment).
    /// </summary>
    [JsonPropertyName("opcode")]
    public required uint Opcode { get; init; }

    /// <summary>
    /// Ordered list of command arguments.
    /// May be empty but is always present.
    /// </summary>
    [JsonPropertyName("arguments")]
    public required IReadOnlyList<CommandArgumentDefinition> Arguments { get; init; }

    /// <summary>
    /// Command priority. Defaults to Low per FPP semantics.
    /// </summary>
    [JsonPropertyName("priority")]
    [JsonConverter(typeof(CommandPriorityConverter))]
    public CommandPriority? Priority { get; init; }

    /// <summary>
    /// Optional printf-style format string.
    /// </summary>
    [JsonPropertyName("format")]
    public string? Format { get; init; }

    /// <summary>
    /// Optional descriptive annotation.
    /// </summary>
    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }

    /// <summary>
    /// Applies the FPP default: priority = low.
    /// </summary>
    [JsonIgnore]
    public CommandPriority EffectivePriority => Priority ?? CommandPriority.Low;
}

public sealed record CommandArgumentDefinition
{
    /// <summary>
    /// Argument name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Argument type descriptor (primitive or qualifiedIdentifier).
    /// </summary>
    [JsonPropertyName("type")]
    public required TypeRef Type { get; init; }

    /// <summary>
    /// Optional annotation for the argument.
    /// </summary>
    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }
}

public enum CommandPriority
{
    Low,
    High
}

public sealed class CommandPriorityConverter
    : JsonConverter<CommandPriority>
{
    public override CommandPriority Read(
        ref Utf8JsonReader reader,
        System.Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "low" => CommandPriority.Low,
            "high" => CommandPriority.High,
            _ => throw new JsonException("Invalid command priority")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        CommandPriority value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            CommandPriority.Low => "low",
            CommandPriority.High => "high",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        });
    }
}
