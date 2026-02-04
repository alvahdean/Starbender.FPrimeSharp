using System;
using System.Collections.Generic;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class BigEndianInt32LengthPrefixedProtocol : IFramedProtocol<LengthPrefixedFrame>
{
    public int? MaxFrameSizeBytes { get; }

    public FrameType SupportedFrameType => FrameType.LengthPrefixedBigEndian32;

    public BigEndianInt32LengthPrefixedProtocol(int? maxFrameSizeBytes = 4 * 1024 * 1024)
        => MaxFrameSizeBytes = maxFrameSizeBytes;

    public bool TryReadFrame(in ReadOnlySequence<byte> buffer,
        out LengthPrefixedFrame frame,
        out SequencePosition consumed,
        out SequencePosition examined)
    {
        frame = default!;
        consumed = buffer.Start;
        examined = buffer.End;

        if (buffer.Length < 4)
        {
            examined = buffer.End;
            return false;
        }

        Span<byte> lenBytes = stackalloc byte[4];
        buffer.Slice(0, 4).CopyTo(lenBytes);
        var length = BinaryPrimitives.ReadInt32BigEndian(lenBytes);

        if (length < 0)
        {
            throw new InvalidDataException("Negative frame length.");
        }

        if (MaxFrameSizeBytes.HasValue && length > MaxFrameSizeBytes.Value)
        {
                    throw new InvalidDataException($"Frame length {length} exceeds limit {MaxFrameSizeBytes.Value}.");
        }

        var total = 4L + length;
        if (buffer.Length < total)
        {
            examined = buffer.End;
            return false;
        }

        var payloadSeq = buffer.Slice(4, length);

        var payload = payloadSeq.ToArray();

        consumed = buffer.GetPosition(total);
        examined = consumed;

        frame = new LengthPrefixedFrame { Payload = payload };

        return true;
    }

    public async ValueTask WriteFrameAsync(PipeWriter writer, LengthPrefixedFrame frame, CancellationToken ct)
    {
        var payload = frame.Payload;
        var length = payload.Length;

        if (MaxFrameSizeBytes.HasValue && length > MaxFrameSizeBytes.Value)
        {
            throw new InvalidDataException($"Frame payload {length} exceeds limit {MaxFrameSizeBytes.Value}.");
        }

        var span = writer.GetSpan(4 + length);

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(0, 4), length);
        payload.Span.CopyTo(span.Slice(4, length));

        writer.Advance(4 + length);

        var flush = await writer.FlushAsync(ct).ConfigureAwait(false);
        if (flush.IsCanceled) ct.ThrowIfCancellationRequested();
    }
}

