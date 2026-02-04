namespace Starbender.FPrimeSharp.Gds.Options;

public sealed class TcpServerOptions
{
    /// <summary>
    /// The local address to listen on. Examples: "0.0.0.0", "127.0.0.1", "::".
    /// </summary>
    public string BindAddress { get; set; } = "0.0.0.0";

    /// <summary>
    /// The local port to listen on.
    /// Use 0 to bind to an ephemeral port (useful for tests).
    /// </summary>
    public int Port { get; set; } = 50000;

    /// <summary>
    /// Max simultaneous accepted connections handled concurrently.
    /// </summary>
    public int MaxConnections { get; set; } = 10;

    /// <summary>
    /// Backlog for the underlying listener.
    /// </summary>
    public int Backlog { get; set; } = 5;

    /// <summary>
    /// PipeReader buffer size used for each connection read loop.
    /// This is NOT the socket receive buffer; it controls the Pipes buffer.
    /// </summary>
    public int ReceiveBufferBytes { get; set; } = 64 * 1024;

    /// <summary>
    /// Optional socket receive buffer size (SO_RCVBUF). 0 = do not set.
    /// </summary>
    public int SocketReceiveBufferBytes { get; set; } = 0;

    /// <summary>
    /// Optional socket send buffer size (SO_SNDBUF). 0 = do not set.
    /// </summary>
    public int SocketSendBufferBytes { get; set; } = 0;

    /// <summary>
    /// If true, disable Nagle for lower latency at the cost of more packets.
    /// </summary>
    public bool NoDelay { get; set; } = true;
}
