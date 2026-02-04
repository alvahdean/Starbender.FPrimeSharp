using System;
using System.Buffers.Binary;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

internal static class LengthPrefix
{
    public static byte[] EncodeBigEndianInt32(byte[] payload)
    {
        var buf = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32BigEndian(buf.AsSpan(0, 4), payload.Length);
        payload.CopyTo(buf, 4);
        return buf;
    }

    public static async Task<byte[]> ReadOneFrameBigEndianInt32Async(NetworkStream stream, CancellationToken ct)
    {
        var header = await ReadExactlyAsync(stream, 4, ct);
        var len = BinaryPrimitives.ReadInt32BigEndian(header);

        if (len < 0) throw new InvalidDataException("Negative length.");

        return await ReadExactlyAsync(stream, len, ct);
    }

    private static async Task<byte[]> ReadExactlyAsync(NetworkStream stream, int count, CancellationToken ct)
    {
        var buf = new byte[count];
        var offset = 0;

        while (offset < count)
        {
            var read = await stream.ReadAsync(buf.AsMemory(offset, count - offset), ct);
            if (read == 0) throw new EndOfStreamException();
            offset += read;
        }

        return buf;
    }
}
