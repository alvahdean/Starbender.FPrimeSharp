using System;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class FPrimePacket
{
    public required FPrimePacketType PacketType { get; init; }
    public required ReadOnlyMemory<byte> Payload { get; init; }
}
