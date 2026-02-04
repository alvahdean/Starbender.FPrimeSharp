using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class FPrimePacketRouter
{
    private readonly IFPrimePacketHandler[] _handlers;

    public FPrimePacketRouter(IEnumerable<IFPrimePacketHandler> handlers)
    {
        _handlers = handlers?.ToArray() ?? Array.Empty<IFPrimePacketHandler>();
    }

    public async ValueTask RouteAsync(FPrimeFrame frame, CancellationToken ct)
    {
        if (frame is null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        var packetType = GetPacketType(frame.Payload);
        var payload = frame.Payload.Length > 0
            ? frame.Payload.Slice(1)
            : ReadOnlyMemory<byte>.Empty;

        var packet = new FPrimePacket
        {
            PacketType = packetType,
            Payload = payload
        };

        foreach (var handler in _handlers)
        {
            if (handler.SupportedPacketTypes.Contains(packetType))
            {
                await handler.HandlePacketAsync(packet, ct).ConfigureAwait(false);
            }
        }
    }

    private static FPrimePacketType GetPacketType(ReadOnlyMemory<byte> payload)
    {
        if (payload.Length == 0)
        {
            return FPrimePacketType.FW_PACKET_UNKNOWN;
        }

        var typeValue = payload.Span[0];
        return Enum.IsDefined(typeof(FPrimePacketType), (int)typeValue)
            ? (FPrimePacketType)typeValue
            : FPrimePacketType.FW_PACKET_UNKNOWN;
    }
}
