using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Starbender.FPrimeSharp.Gds.Protocol;
using Xunit;

namespace Starbender.FPrimeSharp.Gds.Framing;

public sealed class BigEndianInt32LengthPrefixedProtocol_Tests
{
    [Fact]
    public void TryReadFrame_ReturnsFalse_WhenLessThanPrefix()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol();
        var buffer = new ReadOnlySequence<byte>(new byte[3]);

        var result = protocol.TryReadFrame(buffer, out var frame, out var consumed, out var examined);

        result.ShouldBeFalse();
        frame.ShouldBe(default(LengthPrefixedFrame));
        consumed.ShouldBe(buffer.Start);
        examined.ShouldBe(buffer.End);
    }

    [Fact]
    public void TryReadFrame_ReturnsFalse_WhenIncompletePayload()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol();
        var payload = new byte[] { 1, 2, 3, 4 };
        var buffer = new ReadOnlySequence<byte>(BuildFrameBytes(payload, payload.Length + 2));

        var result = protocol.TryReadFrame(buffer, out _, out var consumed, out var examined);

        result.ShouldBeFalse();
        consumed.ShouldBe(buffer.Start);
        examined.ShouldBe(buffer.End);
    }

    [Fact]
    public void TryReadFrame_ParsesFrame_WhenComplete()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol();
        var payload = new byte[] { 10, 11, 12 };
        var buffer = new ReadOnlySequence<byte>(BuildFrameBytes(payload, payload.Length));

        var result = protocol.TryReadFrame(buffer, out var frame, out var consumed, out var examined);

        result.ShouldBeTrue();
        frame.Payload.ToArray().ShouldBe(payload);
        consumed.ShouldBe(buffer.End);
        examined.ShouldBe(buffer.End);
    }

    [Fact]
    public void TryReadFrame_Throws_WhenLengthNegative()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol();
        var buffer = new ReadOnlySequence<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        Should.Throw<InvalidDataException>(() =>
            protocol.TryReadFrame(buffer, out _, out _, out _));
    }

    [Fact]
    public void TryReadFrame_Throws_WhenLengthExceedsMax()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol(maxFrameSizeBytes: 4);
        var payload = new byte[] { 1, 2, 3, 4, 5 };
        var buffer = new ReadOnlySequence<byte>(BuildFrameBytes(payload, payload.Length));

        Should.Throw<InvalidDataException>(() =>
            protocol.TryReadFrame(buffer, out _, out _, out _));
    }

    [Fact]
    public async Task WriteFrameAsync_WritesBigEndianPrefixAndPayload()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol();
        var payload = new byte[] { 7, 8, 9, 10 };
        var frame = new LengthPrefixedFrame { Payload = payload };
        var pipe = new Pipe();

        await protocol.WriteFrameAsync(pipe.Writer, frame, CancellationToken.None);

        var read = await pipe.Reader.ReadAsync();
        var buffer = read.Buffer;
        var bytes = buffer.ToArray();

        bytes.Length.ShouldBe(4 + payload.Length);
        BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(0, 4)).ShouldBe(payload.Length);
        bytes.AsSpan(4, payload.Length).ToArray().ShouldBe(payload);

        pipe.Reader.AdvanceTo(buffer.End);
        pipe.Reader.Complete();
        pipe.Writer.Complete();
    }

    [Fact]
    public async Task WriteFrameAsync_Throws_WhenPayloadExceedsMax()
    {
        var protocol = new BigEndianInt32LengthPrefixedProtocol(maxFrameSizeBytes: 4);
        var payload = new byte[] { 1, 2, 3, 4, 5 };
        var frame = new LengthPrefixedFrame { Payload = payload };

        await Should.ThrowAsync<InvalidDataException>(() =>
            protocol.WriteFrameAsync(new Pipe().Writer, frame, CancellationToken.None).AsTask());
    }

    private static byte[] BuildFrameBytes(byte[] payload, int declaredLength)
    {
        var bytes = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(0, 4), declaredLength);
        payload.CopyTo(bytes.AsSpan(4));
        return bytes;
    }
}
