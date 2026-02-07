using System;
using System.Collections.Generic;
using System.Text;

namespace Starbender.FPrimeSharp.Gds.Topology;

public sealed record TelemetryPacketSetDefinition
{
    public required string Name { get; init; }

    public required IReadOnlyList<TelemetryPacketDefinition> Packets { get; init; }
}

public sealed record TelemetryPacketDefinition
{
    public required string Name { get; init; }

    // Packet-local ID
    public required uint Id { get; init; }

    // CCSDS APID (11-bit, validate 0–2047)
    public required ushort Apid { get; init; }

    // Ordered list of fully-qualified telemetry channel names
    public required IReadOnlyList<string> Channels { get; init; }
}
