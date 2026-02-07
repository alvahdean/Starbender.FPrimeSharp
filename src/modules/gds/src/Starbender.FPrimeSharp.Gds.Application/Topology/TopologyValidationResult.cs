#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace Starbender.FPrimeSharp.Gds.Topology;

public class TopologyValidationResult
{
    /// <summary>
    /// The path in the topology tree for which this validation applies
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Indicates if the validation was successful. If null, then the validation has not been run or completed
    /// </summary>
    public bool? IsValid { get; set; }
    public List<TopologyValidationMessage> Messages { get; } = new();

    public List<TopologyValidationResult> Children { get; } = new();
}

public enum TopologyValidationSeverity
{
    Information,
    Warning,
    Error
}

public class TopologyValidationMessage
{
    public TopologyValidationSeverity Severity { get; set;  }
    public string? Message { get; set; }
}
