using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

/// <summary>
/// Represents entries in "typeDefinitions". The "kind" field indicates which optional fields are present.
/// Examples in the file include kinds "alias", "enum", "array", "struct".:contentReference[oaicite:2]{index=2}:contentReference[oaicite:3]{index=3}:contentReference[oaicite:4]{index=4}
/// </summary>
public sealed class TypeDefinition
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = ""; // alias|enum|array|struct|...

    [JsonPropertyName("qualifiedName")]
    public string QualifiedName { get; set; } = "";

    // ---- Common optional fields ----

    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; }

    /// <summary>
    /// Default value; its JSON shape depends on the kind (scalar, array, object, string enum literal, etc.)
    /// </summary>
    [JsonPropertyName("default")]
    public JsonElement? Default { get; set; }

    // ---- alias ----

    /// <summary>For alias definitions: the alias type reference.</summary>
    [JsonPropertyName("type")]
    public TypeRef? Type { get; set; }

    /// <summary>For alias definitions: the underlying type reference.</summary>
    [JsonPropertyName("underlyingType")]
    public TypeRef? UnderlyingType { get; set; }

    // ---- enum ----

    [JsonPropertyName("representationType")]
    public TypeRef? RepresentationType { get; set; }

    [JsonPropertyName("enumeratedConstants")]
    public List<EnumeratedConstant>? EnumeratedConstants { get; set; }

    // ---- array ----

    /// <summary>Array length.</summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("elementType")]
    public TypeRef? ElementType { get; set; }

    // ---- struct ----

    /// <summary>
    /// Struct members are a JSON object keyed by member name. Each member has a type + index (+ optional size/annotation).:contentReference[oaicite:5]{index=5}
    /// </summary>
    [JsonPropertyName("members")]
    public Dictionary<string, StructMemberDefinition>? Members { get; set; }
}

public sealed class EnumeratedConstant
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("value")]
    public long Value { get; set; }

    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; }
}

/// <summary>
/// Used throughout the dictionary to reference a type.
/// For primitives you’ll see kinds like "integer", "float", "bool", "string".
/// For user-defined types you’ll see kind "qualifiedIdentifier".:contentReference[oaicite:6]{index=6}:contentReference[oaicite:7]{index=7}
/// </summary>
public sealed class TypeRef
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = ""; // integer|float|bool|string|qualifiedIdentifier|...

    /// <summary>Bit width for numeric/bool, or max length for string (as used in this dictionary).</summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    /// <summary>Only present for integer kinds.</summary>
    [JsonPropertyName("signed")]
    public bool? Signed { get; set; }
}
