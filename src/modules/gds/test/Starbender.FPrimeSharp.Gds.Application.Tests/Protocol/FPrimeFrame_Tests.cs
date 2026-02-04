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

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class FPrimeFrame_Tests
{
    [Fact]
    public async Task WriteFrameAsync_WritesValidFrame_ThatReaderParses()
    {
        var payload = new byte[] { 1, 2, 3, 4, 5 };
        var writer = new FPrimeFrameWriter();
        var reader = new FPrimeFrameReader();
        var pipe = new Pipe();

        await writer.WriteFrameAsync(pipe.Writer, new ReadOnlySequence<byte>(payload), CancellationToken.None);

        var read = await pipe.Reader.ReadAsync();
        var buffer = read.Buffer;

        var ok = reader.TryReadFrame(buffer, out var frame, out var consumed, out var examined);

        ok.ShouldBeTrue();
        frame.Payload.ToArray().ShouldBe(payload);
        frame.PayloadLength.ShouldBe(payload.Length);
        frame.StartWord.ShouldBe(FPrimeFrameReader.StartWord);
        frame.Crc.ShouldBeGreaterThan(0u);
        consumed.ShouldBe(buffer.End);
        examined.ShouldBe(buffer.End);

        pipe.Reader.AdvanceTo(consumed);
    }

    [Fact]
    public void TryReadFrame_ReturnsFalse_WhenIncompleteHeader()
    {
        var reader = new FPrimeFrameReader();
        var buffer = new ReadOnlySequence<byte>(new byte[FPrimeFrameReader.HeaderBytes - 1]);

        var ok = reader.TryReadFrame(buffer, out var frame, out var consumed, out var examined);

        ok.ShouldBeFalse();
        frame.ShouldBe(default(FPrimeFrame));
        consumed.ShouldBe(buffer.Start);
        examined.ShouldBe(buffer.End);
    }

    [Fact]
    public void TryReadFrame_ReturnsFalse_WhenIncompletePayload()
    {
        var payload = new byte[] { 10, 11, 12, 13 };
        var bytes = BuildFrameBytes(payload);
        var truncated = new byte[bytes.Length - 2];
        Array.Copy(bytes, truncated, truncated.Length);
        var buffer = new ReadOnlySequence<byte>(truncated);

        var reader = new FPrimeFrameReader();
        var ok = reader.TryReadFrame(buffer, out _, out var consumed, out var examined);

        ok.ShouldBeFalse();
        consumed.ShouldBe(buffer.Start);
        examined.ShouldBe(buffer.End);
    }

    [Fact]
    public void TryReadFrame_Throws_WhenStartWordInvalid()
    {
        var payload = new byte[] { 1, 2, 3 };
        var bytes = BuildFrameBytes(payload);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(0, 4), 0x01020304);

        var reader = new FPrimeFrameReader();
        var buffer = new ReadOnlySequence<byte>(bytes);

        Should.Throw<InvalidDataException>(() =>
            reader.TryReadFrame(buffer, out _, out _, out _));
    }

    [Fact]
    public void TryReadFrame_Throws_WhenCrcInvalid()
    {
        var payload = new byte[] { 1, 2, 3 };
        var bytes = BuildFrameBytes(payload);
        bytes[^1] ^= 0xFF;

        var reader = new FPrimeFrameReader();
        var buffer = new ReadOnlySequence<byte>(bytes);

        Should.Throw<InvalidDataException>(() =>
            reader.TryReadFrame(buffer, out _, out _, out _));
    }

    [Fact]
    public async Task WriteFrameAsync_Throws_WhenFrameTooLarge()
    {
        var payload = new byte[] { 1, 2, 3, 4, 5 };
        var writer = new FPrimeFrameWriter(maxFrameSizeBytes: 10);

        await Should.ThrowAsync<InvalidDataException>(() =>
            writer.WriteFrameAsync(new Pipe().Writer, new ReadOnlySequence<byte>(payload), CancellationToken.None).AsTask());
    }

    private static byte[] BuildFrameBytes(byte[] payload)
    {
        var payloadLength = (uint)payload.Length;
        var totalLength = FPrimeFrameReader.HeaderBytes + payload.Length + FPrimeFrameReader.TrailerBytes;
        var bytes = new byte[totalLength];

        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(0, 4), FPrimeFrameReader.StartWord);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(4, 4), payloadLength);
        payload.CopyTo(bytes.AsSpan(FPrimeFrameReader.HeaderBytes));

        var header = bytes.AsSpan(0, FPrimeFrameReader.HeaderBytes);
        var crc = FPrimeCrc32.Compute(header, payload);
        BinaryPrimitives.WriteUInt32BigEndian(
            bytes.AsSpan(FPrimeFrameReader.HeaderBytes + payload.Length, FPrimeFrameReader.TrailerBytes),
            crc);

        return bytes;
    }
}
