using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public delegate ValueTask<ProtocolExceptionAction> ProtocolExceptionHandler(
    Exception exception,
    CancellationToken ct);

/// <summary>Provides a contract for protocol handlers.</summary>
public interface IProtocolHandler
{
    /// <summary>True after RunAsync starts and before it completes.</summary>
    bool IsRunning { get; }

    /// <summary>
    /// Indicates the handler is currently attached to a stream (not a socket-level truth).
    /// </summary>
    bool IsAttached { get; }

    /// <summary>
    /// Requests the handler to stop processing and return from RunAsync as soon as practical (best-effort).
    /// Does not guarantee the underlying socket/stream is closed.
    /// </summary>
    void Disconnect();

    /// <summary>Begin running the protocol on the specified stream.</summary>
    Task RunAsync(Stream stream, CancellationToken ct);

    /// <summary>
    /// Optional exception callback invoked when unhandled exceptions occur during protocol execution.
    /// </summary>
    ProtocolExceptionHandler? OnExceptionAsync { get; set; }
}
