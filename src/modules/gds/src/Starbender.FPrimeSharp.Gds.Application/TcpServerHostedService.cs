// TcpServerHostedService.cs
using System;
using System.Net;
using System.Net.Sockets;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Starbender.FPrimeSharp.Gds.Options;
using Starbender.FPrimeSharp.Gds.Protocol;
using Volo.Abp.Uow;
using System.IO;

namespace Starbender.FPrimeSharp.Gds;

/// <summary>
/// Generic TCP server hosted service that accepts connections and runs a protocol handler per connection.
/// - Uses ABP DI scope per connection
/// - Enforces MaxConnections
/// - Supports Port=0 ephemeral binding (BoundPort is set)
/// </summary>
public abstract class TcpServerHostedService<TProtocol> : BackgroundService
    where TProtocol : class, IProtocolHandler
{
    protected IServiceScopeFactory ScopeFactory { get; }
    protected ILogger Logger { get; }
    protected TcpServerOptions ServerOptions { get; }

    private TcpListener? _listener;
    private SemaphoreSlim? _connectionGate;

    /// <summary>
    /// Actual bound port after Start(). If Port=0, this is the ephemeral port assigned by the OS.
    /// </summary>
    public int BoundPort { get; private set; }

    protected TcpServerHostedService(
        IServiceScopeFactory scopeFactory,
        ILoggerFactory loggerFactory,
        IOptions<TcpServerOptions> options)
    {
        ScopeFactory = scopeFactory;
        ServerOptions = options.Value;
        Logger = loggerFactory.CreateLogger(LoggerName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ip = IPAddress.Parse(ServerOptions.BindAddress);
        _listener = new TcpListener(ip, ServerOptions.Port);
        _listener.Start(ServerOptions.Backlog);

        BoundPort = ((IPEndPoint)_listener.LocalEndpoint).Port;

        _connectionGate = new SemaphoreSlim(ServerOptions.MaxConnections, ServerOptions.MaxConnections);

        Logger.LogInformation(
            "Listening on {Host}:{Port} (requested port: {RequestedPort}, max connections: {MaxConnections})",
            ServerOptions.BindAddress,
            BoundPort,
            ServerOptions.Port,
            ServerOptions.MaxConnections);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(stoppingToken).ConfigureAwait(false);

                await _connectionGate.WaitAsync(stoppingToken).ConfigureAwait(false);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await HandleClientAsync(client, stoppingToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        try { _connectionGate.Release(); } catch { /* ignore */ }
                    }
                });
            }
        } catch (OperationCanceledException)
        {
            // normal shutdown
        } catch (Exception ex)
        {
            Logger.LogError(ex, "Fatal error in accept loop.");
            throw;
        }
        finally
        {
            try { _listener.Stop(); } catch { /* ignore */ }

            _connectionGate?.Dispose();
            _connectionGate = null;
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try { _listener?.Stop(); } catch { /* ignore */ }

        return base.StopAsync(cancellationToken);
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken stoppingToken)
    {
        using (client)
        {
            await using var scope = ScopeFactory.CreateAsyncScope();

            ApplySocketOptions(client);

            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            try
            {
                using var uow = uowManager.Begin(requiresNew: true);

                using var stream = client.GetStream();

                // Run the protocol handler (resolved per-connection from DI).
                // If you want the protocol handler to be singleton/shared, register it as singleton
                // and use GetRequiredService<TProtocol>(); still safe because each connection has its own stream.
                var protocol = scope.ServiceProvider.GetRequiredService<TProtocol>();

                await RunProtocolAsync(protocol, stream, stoppingToken).ConfigureAwait(false);

                await uow.CompleteAsync(stoppingToken).ConfigureAwait(false);
            } catch (Exception ex)
            {
                Logger.LogWarning(ex, "Client handling failed.");
            }
        }
    }

    protected virtual ValueTask RunProtocolAsync(TProtocol protocol, Stream stream, CancellationToken ct)
    {
        // Default: handler owns its own Pipes creation, as in your ProtocolHandlerBase.
        // If your IProtocolHandler uses PipeReader/PipeWriter directly, override this method.
        return new ValueTask(protocol.RunAsync(stream, ct));
    }

    protected virtual void ApplySocketOptions(TcpClient client)
    {
        try
        {
            client.NoDelay = ServerOptions.NoDelay;

            if (ServerOptions.SocketReceiveBufferBytes > 0)
                client.ReceiveBufferSize = ServerOptions.SocketReceiveBufferBytes;

            if (ServerOptions.SocketSendBufferBytes > 0)
                client.SendBufferSize = ServerOptions.SocketSendBufferBytes;
        } catch (Exception ex)
        {
            Logger.LogDebug(ex, "Failed to apply socket options.");
        }
    }

    private string LoggerName
    {
        get
        {
            var protocol = typeof(TProtocol).Name?.Replace("Handler", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            return $"TCPServer:{protocol}";
        }
    }
}
