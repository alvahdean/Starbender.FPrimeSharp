using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

/// <summary>
/// Contract for frame writers
/// </summary>
/// <typeparam name="TFrame">The datatype describing the frame</typeparam>
public interface IFrameWriter
{
    /// <summary>
    /// Writes a frame to the output pipe.
    /// Implementations should write the full encoded frame and flush.
    /// </summary>
    ValueTask WriteFrameAsync(PipeWriter writer, ReadOnlySequence<byte> payload, CancellationToken ct);
}
