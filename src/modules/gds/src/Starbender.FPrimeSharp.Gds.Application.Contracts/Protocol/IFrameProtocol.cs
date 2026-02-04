namespace Starbender.FPrimeSharp.Gds.Protocol;

/// <summary>
/// Contract for a full frame protocol handler
/// </summary>
/// <typeparam name="TFrame">The datatype describing the frame</typeparam>
public interface IFramedProtocol<TFrame> : IFrameReader<TFrame>, IFrameWriter<TFrame>
{
    /// <summary>
    /// Indicates the frame type that the protocol implements
    /// </summary>
    FrameType SupportedFrameType { get; }
    
    /// <summary>
    /// Optional max frame size guard (implementations may enforce).
    /// </summary>
    int? MaxFrameSizeBytes { get; }
}
