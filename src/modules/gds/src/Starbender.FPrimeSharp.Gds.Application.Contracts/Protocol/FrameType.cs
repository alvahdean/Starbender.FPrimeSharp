namespace Starbender.FPrimeSharp.Gds.Protocol;

public enum FrameType
{
    Unspecified = 0,
    LengthPrefixedBigEndian32, // Generic frame with length prefix
    LengthPrefixedLittleEndian32, // Generic frame with length prefix
    CCSDSTM,    // Telemetry Data
    CCSDSTC,    // Commands
}
