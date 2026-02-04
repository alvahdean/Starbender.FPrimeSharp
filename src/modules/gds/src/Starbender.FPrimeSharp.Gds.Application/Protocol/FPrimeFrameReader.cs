using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;

namespace Starbender.FPrimeSharp.Gds.Protocol;

/// <summary>
/// Parses F Prime frames from a byte buffer.
/// </summary>
public sealed class FPrimeFrameReader : IFrameReader<FPrimeFrame>
{
    public const uint StartWord = 0xDEADBEEF;
    public const int HeaderBytes = 8;
    public const int TrailerBytes = 4;

    public int? MaxFrameSizeBytes { get; }

    public FPrimeFrameReader(int? maxFrameSizeBytes = 4 * 1024 * 1024)
    {
        MaxFrameSizeBytes = maxFrameSizeBytes;
    }

    public bool TryReadFrame(
        in ReadOnlySequence<byte> buffer,
        out FPrimeFrame frame,
        out SequencePosition consumed,
        out SequencePosition examined)
    {
        frame = default!;
        consumed = buffer.Start;
        examined = buffer.End;

        if (buffer.Length < HeaderBytes + TrailerBytes)
        {
            return false;
        }

        Span<byte> header = stackalloc byte[HeaderBytes];
        buffer.Slice(0, HeaderBytes).CopyTo(header);

        var startWord = BinaryPrimitives.ReadUInt32BigEndian(header.Slice(0, 4));
        if (startWord != StartWord)
        {
            throw new InvalidDataException($"Invalid FPrime start word: 0x{startWord:X8}.");
        }

        var payloadLength = BinaryPrimitives.ReadUInt32BigEndian(header.Slice(4, 4));
        var totalLength = HeaderBytes + (long)payloadLength + TrailerBytes;

        if (MaxFrameSizeBytes.HasValue && totalLength > MaxFrameSizeBytes.Value)
        {
            throw new InvalidDataException(
                $"FPrime frame length {totalLength} exceeds limit {MaxFrameSizeBytes.Value}.");
        }

        if (buffer.Length < totalLength)
        {
            return false;
        }

        var payloadSeq = buffer.Slice(HeaderBytes, payloadLength);
        var payload = payloadSeq.ToArray();

        Span<byte> crcBytes = stackalloc byte[4];
        buffer.Slice(HeaderBytes + payloadLength, TrailerBytes).CopyTo(crcBytes);
        var crc = BinaryPrimitives.ReadUInt32BigEndian(crcBytes);

        var computedCrc = FPrimeCrc32.Compute(header, payload);
        if (crc != computedCrc)
        {
            throw new InvalidDataException(
                $"FPrime frame CRC mismatch. Expected 0x{crc:X8}, computed 0x{computedCrc:X8}.");
        }

        consumed = buffer.GetPosition(totalLength);
        examined = consumed;

        if (payloadLength > int.MaxValue)
        {
            throw new InvalidDataException(
                $"FPrime payload length {payloadLength} exceeds supported size.");
        }

        frame = new FPrimeFrame
        {
            Payload = payload,
            IsFramed = true,
            StartWord = startWord,
            PayloadLength = (int)payloadLength,
            Crc = crc
        };

        return true;
    }

}
