#nullable enable
using System;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed record TelemetryChannelDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("type")]
    public required TypeRef Type { get; init; }

    // In the Ref dictionary this is an integer; keep as uint to match typical F´ ID domains.
    [JsonPropertyName("id")]
    public required uint Id { get; init; }

    // In the JSON dictionary this is a closed vocabulary: "always" | "on change".
    // If omitted, FPP semantics default to "always" (apply the default during validation/post-processing).
    [JsonPropertyName("telemetryUpdate")]
    [JsonConverter(typeof(TelemetryUpdateModeConverter))]
    public TelemetryUpdateMode? TelemetryUpdate { get; init; }

    // Optional metadata
    [JsonPropertyName("format")]
    public string? Format { get; init; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; init; }

    // Optional limits metadata. Keys are NOT arbitrary; they are the fixed set: yellow/orange/red.
    [JsonPropertyName("limits")]
    public LimitsDefinition? Limits { get; init; }

    /// <summary>
    /// Convenience view applying the FPP default rule:
    /// if telemetryUpdate is missing, treat as Always.
    /// </summary>
    [JsonIgnore]
    public TelemetryUpdateMode EffectiveTelemetryUpdate => TelemetryUpdate ?? TelemetryUpdateMode.Always;
}

public enum TelemetryUpdateMode
{
    Always,
    OnChange
}

public sealed class TelemetryUpdateModeConverter : System.Text.Json.Serialization.JsonConverter<TelemetryUpdateMode>
{
    public override TelemetryUpdateMode Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return s switch
        {
            "always" => TelemetryUpdateMode.Always,
            "on change" => TelemetryUpdateMode.OnChange,
            _ => throw new System.Text.Json.JsonException($"Unknown telemetryUpdate value: '{s}'")
        };
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, TelemetryUpdateMode value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            TelemetryUpdateMode.Always => "always",
            TelemetryUpdateMode.OnChange => "on change",
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        });
    }
}

/// <summary>
/// Limits metadata. These are not arbitrary dictionary keys; the schema is fixed.
/// </summary>
public sealed record LimitsDefinition
{
    [JsonPropertyName("low")]
    public LimitBand? Low { get; init; }

    [JsonPropertyName("high")]
    public LimitBand? High { get; init; }
}

/// <summary>
/// A limit band is a fixed set of optional thresholds.
/// These labels are a closed vocabulary (yellow/orange/red).
/// </summary>
public sealed record LimitBand
{
    [JsonPropertyName("yellow")]
    public double? Yellow { get; init; }

    [JsonPropertyName("orange")]
    public double? Orange { get; init; }

    [JsonPropertyName("red")]
    public double? Red { get; init; }
}

/// <summary>
/// Optional helper you can use in the GDS layer to interpret limits.
/// The dictionary provides thresholds; interpretation is GDS-defined.
/// </summary>
public enum LimitSeverity
{
    Yellow,
    Orange,
    Red
}
