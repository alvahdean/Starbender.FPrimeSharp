using System;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public static class FPrimeCrc32
{
    private const uint Polynomial = 0xEDB88320u;
    private static readonly uint[] Table = CreateTable();

    public static uint Compute(ReadOnlySpan<byte> header, ReadOnlySpan<byte> payload)
    {
        var crc = 0xFFFFFFFFu;
        crc = Update(crc, header);
        crc = Update(crc, payload);
        return ~crc;
    }

    private static uint Update(uint crc, ReadOnlySpan<byte> data)
    {
        foreach (var b in data)
        {
            var tmp = (crc ^ b) & 0xFF;
            crc = (crc >> 8) ^ Table[tmp];
        }

        return crc;
    }

    private static uint[] CreateTable()
    {
        var table = new uint[256];
        for (uint i = 0; i < table.Length; i++)
        {
            var crc = i;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) == 1 ? (crc >> 1) ^ Polynomial : crc >> 1;
            }

            table[i] = crc;
        }

        return table;
    }
}