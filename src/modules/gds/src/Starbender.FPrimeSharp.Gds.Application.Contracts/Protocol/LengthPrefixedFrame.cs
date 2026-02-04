using System;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class LengthPrefixedFrame
{
    public required ReadOnlyMemory<byte> Payload { get; init; }
}
