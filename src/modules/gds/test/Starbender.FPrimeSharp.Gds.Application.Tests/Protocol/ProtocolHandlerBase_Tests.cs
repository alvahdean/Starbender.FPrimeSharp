using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shouldly;
using Starbender.FPrimeSharp.Gds.Options;
using Starbender.FPrimeSharp.Gds.Protocol;
using Xunit;

namespace Starbender.FPrimeSharp.Gds.Protocol;

public sealed class ProtocolHandlerBase_Tests
{
    [Fact]
    public void EnsureAttached_Throws_WhenNotAttached()
    {
        var handler = new TestHandler(CreateOptions(), (_, _, _) => Task.CompletedTask);

        Should.Throw<InvalidOperationException>(() => handler.EnsureAttachedPublic());
    }

    [Fact]
    public async Task RunAsync_SetsRunningAndAttached_AndResetsOnCompletion()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowExit = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var handler = new TestHandler(
            CreateOptions(),
            async (_, _, _) =>
            {
                started.TrySetResult();
                await allowExit.Task;
            });

        var runTask = handler.RunAsync(new MemoryStream(), CancellationToken.None);

        await started.Task;
        handler.IsRunning.ShouldBeTrue();
        handler.IsAttached.ShouldBeTrue();

        allowExit.TrySetResult();
        await runTask;

        handler.IsRunning.ShouldBeFalse();
        handler.IsAttached.ShouldBeFalse();
    }

    [Fact]
    public async Task RunAsync_InvokesExceptionHandler_AndSwallows_WhenCloseGracefully()
    {
        var handler = new TestHandler(
            CreateOptions(),
            (_, _, _) => throw new InvalidOperationException("boom"));

        var called = false;
        handler.OnExceptionAsync = (ex, ct) =>
        {
            called = true;
            ex.ShouldBeOfType<InvalidOperationException>();
            return new ValueTask<ProtocolExceptionAction>(ProtocolExceptionAction.CloseGracefully);
        };

        await handler.RunAsync(new MemoryStream(), CancellationToken.None);

        called.ShouldBeTrue();
        handler.IsRunning.ShouldBeFalse();
        handler.IsAttached.ShouldBeFalse();
    }

    [Fact]
    public async Task RunAsync_Rethrows_WhenExceptionHandlerReturnsAbort()
    {
        var handler = new TestHandler(
            CreateOptions(),
            (_, _, _) => throw new InvalidOperationException("boom"));

        handler.OnExceptionAsync = (_, _) =>
            new ValueTask<ProtocolExceptionAction>(ProtocolExceptionAction.Abort);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.RunAsync(new MemoryStream(), CancellationToken.None));
    }

    [Fact]
    public async Task Disconnect_CancelsRunCore_AndSetsCloseRequested()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var handler = new TestHandler(
            CreateOptions(),
            async (_, _, ct) =>
            {
                started.TrySetResult();
                try
                {
                    await Task.Delay(Timeout.Infinite, ct);
                }
                catch (OperationCanceledException)
                {
                    cancelled.TrySetResult();
                }
            });

        var runTask = handler.RunAsync(new MemoryStream(), CancellationToken.None);

        await started.Task;
        handler.CloseRequestedPublic.ShouldBeFalse();

        handler.Disconnect();

        await cancelled.Task;
        handler.CloseRequestedPublic.ShouldBeTrue();

        await runTask;
        handler.IsRunning.ShouldBeFalse();
    }

    private static IOptions<TcpServerOptions> CreateOptions()
    {
        return new OptionsWrapper<TcpServerOptions>(new TcpServerOptions
        {
            ReceiveBufferBytes = 256
        });
    }

    private sealed class TestHandler : ProtocolHandlerBase
    {
        private readonly Func<PipeReader, PipeWriter, CancellationToken, Task> _runCore;

        public TestHandler(
            IOptions<TcpServerOptions> options,
            Func<PipeReader, PipeWriter, CancellationToken, Task> runCore)
            : base(options)
        {
            _runCore = runCore;
        }

        public bool CloseRequestedPublic => CloseRequested;

        public void EnsureAttachedPublic() => EnsureAttached();

        protected override Task RunCoreAsync(
            PipeReader reader,
            PipeWriter writer,
            CancellationToken ct)
            => _runCore(reader, writer, ct);
    }
}
