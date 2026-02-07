#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed record EventDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    // Examples: "ACTIVITY_HI", "ACTIVITY_LO", "WARNING_LO", "WARNING_HI", "DIAGNOSTIC":contentReference[oaicite:3]{index=3}:contentReference[oaicite:4]{index=4}
    [JsonPropertyName("severity")]
    [JsonConverter(typeof(EventSeverityConverter))]
    public required EventSeverity Severity { get; init; }

    // Always present in this file (sometimes empty):contentReference[oaicite:5]{index=5}
    [JsonPropertyName("formalParams")]
    public required IReadOnlyList<EventFormalParamDefinition> FormalParams { get; init; }

    [JsonPropertyName("id")]
    public required uint Id { get; init; }

    [JsonPropertyName("format")]
    public required string Format { get; init; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }

    // Optional in this file:contentReference[oaicite:6]{index=6}
    [JsonPropertyName("throttle")]
    public EventThrottleDefinition? Throttle { get; init; }
}

public sealed record EventFormalParamDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("type")]
    public required TypeRef Type { get; init; }

    // Present in this file for params:contentReference[oaicite:7]{index=7}
    [JsonPropertyName("ref")]
    public required bool Ref { get; init; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }
}

public sealed record EventThrottleDefinition
{
    [JsonPropertyName("count")]
    public required int Count { get; init; }

    // Appears as null in the file for some events:contentReference[oaicite:8]{index=8}
    [JsonPropertyName("every")]
    public int? Every { get; init; }
}

public enum EventSeverity
{
    WarningHi,
    WarningLo,
    Command,
    ActivityHi,
    ActivityLo,
    Diagnostic
}

public sealed class EventSeverityConverter : JsonConverter<EventSeverity>
{
    public override EventSeverity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return s switch
        {
            "WARNING_HI" => EventSeverity.WarningHi,
            "WARNING_LO" => EventSeverity.WarningLo,
            "COMMAND" => EventSeverity.Command,
            "ACTIVITY_HI" => EventSeverity.ActivityHi,
            "ACTIVITY_LO" => EventSeverity.ActivityLo,
            "DIAGNOSTIC" => EventSeverity.Diagnostic,
            _ => throw new JsonException($"Unknown event severity: '{s}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, EventSeverity value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            EventSeverity.WarningHi => "WARNING_HI",
            EventSeverity.WarningLo => "WARNING_LO",
            EventSeverity.Command => "COMMAND",
            EventSeverity.ActivityHi => "ACTIVITY_HI",
            EventSeverity.ActivityLo => "ACTIVITY_LO",
            EventSeverity.Diagnostic => "DIAGNOSTIC",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        });
    }
}
