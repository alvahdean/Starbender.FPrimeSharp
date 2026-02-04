using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

/// <summary>
/// Writes F Prime frames to a pipe.
/// </summary>
public sealed class FPrimeFrameWriter : IFrameWriter
{
    public const uint StartWord = FPrimeFrameReader.StartWord;
    public const int HeaderBytes = FPrimeFrameReader.HeaderBytes;
    public const int TrailerBytes = FPrimeFrameReader.TrailerBytes;

    public int? MaxFrameSizeBytes { get; }

    public FPrimeFrameWriter(int? maxFrameSizeBytes = 4 * 1024 * 1024)
    {
        MaxFrameSizeBytes = maxFrameSizeBytes;
    }

    public async ValueTask WriteFrameAsync(
        PipeWriter writer,
        ReadOnlySequence<byte> payload,
        CancellationToken ct)
    {
        var payloadLength = payload.Length;
        if (payloadLength > uint.MaxValue)
        {
            throw new InvalidDataException(
                $"FPrime payload length {payloadLength} exceeds supported size.");
        }

        var totalLength = HeaderBytes + (long)payloadLength + TrailerBytes;
        if (MaxFrameSizeBytes.HasValue && totalLength > MaxFrameSizeBytes.Value)
        {
            throw new InvalidDataException(
                $"FPrime frame length {totalLength} exceeds limit {MaxFrameSizeBytes.Value}.");
        }

        if (totalLength > int.MaxValue)
        {
            throw new InvalidDataException(
                $"FPrime frame length {totalLength} exceeds supported size.");
        }

        Span<byte> header = stackalloc byte[HeaderBytes];
        BinaryPrimitives.WriteUInt32BigEndian(header.Slice(0, 4), StartWord);
        BinaryPrimitives.WriteUInt32BigEndian(header.Slice(4, 4), (uint)payloadLength);

        uint crc;
        if (payload.IsSingleSegment)
        {
            crc = FPrimeCrc32.Compute(header, payload.FirstSpan);
        }
        else
        {
            var payloadArray = payload.ToArray();
            crc = FPrimeCrc32.Compute(header, payloadArray);
        }

        var span = writer.GetSpan((int)totalLength);
        header.CopyTo(span.Slice(0, HeaderBytes));

        var payloadSpan = span.Slice(HeaderBytes, (int)payloadLength);
        payload.CopyTo(payloadSpan);

        BinaryPrimitives.WriteUInt32BigEndian(
            span.Slice(HeaderBytes + (int)payloadLength, TrailerBytes),
            crc);

        writer.Advance((int)totalLength);

        var flush = await writer.FlushAsync(ct).ConfigureAwait(false);
        if (flush.IsCanceled) ct.ThrowIfCancellationRequested();
    }
}
