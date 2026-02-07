using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed class DictionaryMetadata
{
    /// <summary>
    /// Fully qualified name of the topology/deployment.
    /// </summary>
    [JsonPropertyName("deploymentName")]
    public required string DeploymentName { get; init; }

    /// <summary>
    /// F´ framework version (documented as semantic-versioning string).
    /// </summary>
    [JsonPropertyName("frameworkVersion")]
    public required string FrameworkVersion { get; init; }

    /// <summary>
    /// Project version (documented as semantic-versioning string).
    /// </summary>
    [JsonPropertyName("projectVersion")]
    public required string ProjectVersion { get; init; }

    /// <summary>
    /// Versions of libraries used by the project (strings).
    /// </summary>
    [JsonPropertyName("libraryVersions")]
    public required IReadOnlyList<string> LibraryVersions { get; init; }

    /// <summary>
    /// JSON dictionary specification version.
    /// </summary>
    [JsonPropertyName("dictionarySpecVersion")]
    public required string DictionarySpecVersion { get; init; }
}

