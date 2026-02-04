using System;

namespace Starbender.FPrimeSharp.Gds;

public interface IFprimeFramer
{
    /// <summary>
    /// Push bytes onto the Framer queue
    /// </summary>
    /// <param name="data"></param>
    void Push(ReadOnlySpan<byte> data);

    /// <summary>
    /// Try to read a full frame from the queue
    /// </summary>
    /// <param name="frame">populated the current state</param>
    /// <returns>true if a frame was dequeued</returns>
    bool TryPopFrame(out ReadOnlyMemory<byte> frame);

    /// <summary>
    /// Optional: for outbound framing. For TC uplink, usually already framed.
    /// </summary>
    ReadOnlyMemory<byte> FrameForSend(ReadOnlyMemory<byte> payloadOrFrame);
}