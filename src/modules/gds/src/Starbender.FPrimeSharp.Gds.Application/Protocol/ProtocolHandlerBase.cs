using Microsoft.Extensions.Options;
using Starbender.FPrimeSharp.Gds.Options;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public abstract class ProtocolHandlerBase : IProtocolHandler
{
    private int _isRunning;
    private int _closeRequested;
    private int _isAttached;

    private CancellationTokenSource? _internalCts;
    protected TcpServerOptions ServerOptions { get; }

    protected ProtocolHandlerBase(IOptions<TcpServerOptions> options)
    {
        ServerOptions = options.Value;
    }

    public bool IsRunning => Volatile.Read(ref _isRunning) == 1;

    // Semantics: active while handler owns a running loop; NOT a socket-level truth.
    public bool IsAttached => Volatile.Read(ref _isAttached) == 1;

    public ProtocolExceptionHandler? OnExceptionAsync { get; set; }

    public void Disconnect()
    {
        Interlocked.Exchange(ref _closeRequested, 1);
        _internalCts?.Cancel(); // ensures ReadAsync unblocks even if caller doesn't cancel ct
    }

    protected bool CloseRequested => Volatile.Read(ref _closeRequested) == 1;

    public async Task RunAsync(Stream stream, CancellationToken ct)
    {
        if (Interlocked.Exchange(ref _isRunning, 1) == 1)
        {
            throw new InvalidOperationException("Handler already running.");
        }

        Interlocked.Exchange(ref _closeRequested, 0);

        _internalCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var linkedCt = _internalCts.Token;

        PipeReader? reader = null;
        PipeWriter? writer = null;

        try
        {
            reader = PipeReader.Create(
                stream,
                new StreamPipeReaderOptions(minimumReadSize: ServerOptions.ReceiveBufferBytes));

            writer = PipeWriter.Create(stream);

            Interlocked.Exchange(ref _isAttached, 1);

            await RunCoreAsync(reader, writer, linkedCt).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var action = await HandleExceptionAsync(ex, linkedCt).ConfigureAwait(false);

            if (action == ProtocolExceptionAction.Abort)
            {
                throw;
            }
        }
        finally
        {
            Interlocked.Exchange(ref _isAttached, 0);

            _internalCts?.Dispose();
            _internalCts = null;

            try
            {
                if (reader != null)
                {
                    await reader.CompleteAsync().ConfigureAwait(false);
                }
            }
            catch { }

            try
            {
                if (writer != null)
                {
                    await writer.CompleteAsync().ConfigureAwait(false);
                }
            }
            catch { }

            Interlocked.Exchange(ref _isRunning, 0);
        }
    }

    protected abstract Task RunCoreAsync(PipeReader reader, PipeWriter writer, CancellationToken ct);

    protected async ValueTask<ProtocolExceptionAction> HandleExceptionAsync(Exception ex, CancellationToken ct)
    {
        if (OnExceptionAsync is null)
        {
            return ProtocolExceptionAction.CloseGracefully;
        }

        return await OnExceptionAsync(ex, ct).ConfigureAwait(false);
    }

    protected void EnsureAttached()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("Handler is not attached to a stream.");
        }
    }
}
