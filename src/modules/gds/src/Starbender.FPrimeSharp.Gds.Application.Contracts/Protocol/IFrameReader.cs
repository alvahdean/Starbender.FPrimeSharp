using System;
using System.Buffers;

namespace Starbender.FPrimeSharp.Gds.Protocol;

/// <summary>
/// Contract for frame readers
/// </summary>
/// <typeparam name="TFrame">The datatype describing the frame</typeparam>
public interface IFrameReader<TFrame>
{
    /// <summary>
    /// Attempts to parse a single frame from the buffer.
    /// Return true if a frame was produced; advance consumed/examined appropriately.
    /// Return false if more data is needed; do not consume incomplete frame bytes.
    /// </summary>
    bool TryReadFrame(in ReadOnlySequence<byte> buffer,
                      out TFrame frame,
                      out SequencePosition consumed,
                      out SequencePosition examined);
}
