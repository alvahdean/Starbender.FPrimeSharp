using System;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class FPrimeFrame
{
    /// <summary>
    /// The length of the payload
    /// </summary>
    public int PayloadLength { get; init; }

    /// <summary>
    /// Payload bytes for the frame.
    /// </summary>
    public required ReadOnlyMemory<byte> Payload { get; init; }

    /// <summary>
    /// If true, Payload is already framed and will be sent as-is.
    /// </summary>
    public bool IsFramed { get; init; }

    /// <summary>
    /// F Prime start word (expected 0xDEADBEEF for framed data).
    /// </summary>
    public uint StartWord { get; init; }

    /// <summary>
    /// CRC from the frame trailer.
    /// </summary>
    public uint Crc { get; init; }
}
