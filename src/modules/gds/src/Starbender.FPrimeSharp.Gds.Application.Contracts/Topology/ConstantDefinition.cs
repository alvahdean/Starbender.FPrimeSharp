using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed class ConstantDefinition
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "constant";

    [JsonPropertyName("qualifiedName")]
    public string QualifiedName { get; set; } = "";

    [JsonPropertyName("type")]
    public TypeRef Type { get; set; } = new();

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; }
}

