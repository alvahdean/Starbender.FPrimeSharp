using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Starbender.FPrimeSharp.Gds.Protocol;
using Xunit;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class FPrimePacketRouter_Tests
{
    [Fact]
    public async Task RouteAsync_RoutesToHandlersThatSupportPacketType()
    {
        var commandHandler = new TestHandler(FPrimePacketType.FW_PACKET_COMMAND);
        var telemHandler = new TestHandler(FPrimePacketType.FW_PACKET_TELEM);
        var router = new FPrimePacketRouter(new[] { commandHandler, telemHandler });

        var frame = new FPrimeFrame
        {
            Payload = new byte[]
            {
                (byte)FPrimePacketType.FW_PACKET_COMMAND,
                0xAA,
                0xBB
            }
        };

        await router.RouteAsync(frame, CancellationToken.None);

        commandHandler.ReceivedPackets.Count.ShouldBe(1);
        telemHandler.ReceivedPackets.Count.ShouldBe(0);

        var packet = commandHandler.ReceivedPackets.Single();
        packet.PacketType.ShouldBe(FPrimePacketType.FW_PACKET_COMMAND);
        packet.Payload.ToArray().ShouldBe(new byte[] { 0xAA, 0xBB });
    }

    [Fact]
    public async Task RouteAsync_UsesUnknown_WhenTypeIsNotDefined()
    {
        var unknownHandler = new TestHandler(FPrimePacketType.FW_PACKET_UNKNOWN);
        var router = new FPrimePacketRouter(new[] { unknownHandler });

        var frame = new FPrimeFrame
        {
            Payload = new byte[] { 0xFE, 0x10 }
        };

        await router.RouteAsync(frame, CancellationToken.None);

        unknownHandler.ReceivedPackets.Count.ShouldBe(1);
        var packet = unknownHandler.ReceivedPackets.Single();
        packet.PacketType.ShouldBe(FPrimePacketType.FW_PACKET_UNKNOWN);
        packet.Payload.ToArray().ShouldBe(new byte[] { 0x10 });
    }

    [Fact]
    public async Task RouteAsync_UsesUnknown_WhenPayloadEmpty()
    {
        var unknownHandler = new TestHandler(FPrimePacketType.FW_PACKET_UNKNOWN);
        var router = new FPrimePacketRouter(new[] { unknownHandler });

        var frame = new FPrimeFrame
        {
            Payload = ReadOnlyMemory<byte>.Empty
        };

        await router.RouteAsync(frame, CancellationToken.None);

        unknownHandler.ReceivedPackets.Count.ShouldBe(1);
        var packet = unknownHandler.ReceivedPackets.Single();
        packet.PacketType.ShouldBe(FPrimePacketType.FW_PACKET_UNKNOWN);
        packet.Payload.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public async Task RouteAsync_RoutesToAllHandlersThatSupportType()
    {
        var handlerA = new TestHandler(FPrimePacketType.FW_PACKET_LOG);
        var handlerB = new TestHandler(FPrimePacketType.FW_PACKET_LOG);
        var router = new FPrimePacketRouter(new[] { handlerA, handlerB });

        var frame = new FPrimeFrame
        {
            Payload = new byte[] { (byte)FPrimePacketType.FW_PACKET_LOG, 0x01 }
        };

        await router.RouteAsync(frame, CancellationToken.None);

        handlerA.ReceivedPackets.Count.ShouldBe(1);
        handlerB.ReceivedPackets.Count.ShouldBe(1);
    }

    private sealed class TestHandler : IFPrimePacketHandler
    {
        private readonly HashSet<FPrimePacketType> _supported;

        public TestHandler(params FPrimePacketType[] supported)
        {
            _supported = supported.Length == 0
                ? new HashSet<FPrimePacketType>()
                : new HashSet<FPrimePacketType>(supported);
        }

        public IReadOnlyCollection<FPrimePacketType> SupportedPacketTypes => _supported;

        public List<FPrimePacket> ReceivedPackets { get; } = new();

        public ValueTask HandlePacketAsync(FPrimePacket packet, CancellationToken ct)
        {
            ReceivedPackets.Add(packet);
            return ValueTask.CompletedTask;
        }
    }
}
