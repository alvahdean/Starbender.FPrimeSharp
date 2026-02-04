using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Starbender.FPrimeSharp.Gds.Options;
using Starbender.FPrimeSharp.Gds.Protocol;
using Volo.Abp.Uow;
using Xunit;

namespace Starbender.FPrimeSharp.Gds;

public sealed class TcpServerHostedService_Tests
{
    [Fact]
    public async Task StartAsync_BindsEphemeralPort_HandlesClientAndCompletesUnitOfWork()
    {
        var protocol = new TestProtocol();
        var uow = Substitute.For<IUnitOfWork>();
        var uowManager = Substitute.For<IUnitOfWorkManager>();
        uowManager
            .Begin(Arg.Any<AbpUnitOfWorkOptions>(), Arg.Any<bool>())
            .Returns(uow);

        var services = new ServiceCollection();
        services.AddSingleton(uowManager);
        services.AddSingleton(protocol);
        using var serviceProvider = services.BuildServiceProvider();

        var options = new OptionsWrapper<TcpServerOptions>(new TcpServerOptions
        {
            BindAddress = "127.0.0.1",
            Port = 0,
            MaxConnections = 1,
            Backlog = 1
        });

        var server = new TestTcpServerHostedService(
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            NullLoggerFactory.Instance,
            options);

        try
        {
            await server.StartAsync(CancellationToken.None);
            await WaitUntilAsync(() => server.BoundPort > 0, TimeSpan.FromSeconds(2));

            server.BoundPort.ShouldBeGreaterThan(0);

            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, server.BoundPort);

            await protocol.WaitForRunAsync(TimeSpan.FromSeconds(2));
            server.ApplySocketOptionsCalled.ShouldBeTrue();

            protocol.AllowComplete();
            await protocol.WaitForCompletionAsync(TimeSpan.FromSeconds(2));

            uowManager.Received(1).Begin(Arg.Any<AbpUnitOfWorkOptions>(), true);
            await uow.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
        }
        finally
        {
            await server.StopAsync(CancellationToken.None);
        }
    }

    private static async Task WaitUntilAsync(Func<bool> predicate, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (!predicate())
        {
            if (DateTime.UtcNow - start > timeout)
            {
                throw new TimeoutException("Condition was not met within the timeout.");
            }

            await Task.Delay(20);
        }
    }

    private sealed class TestTcpServerHostedService : TcpServerHostedService<TestProtocol>
    {
        public bool ApplySocketOptionsCalled { get; private set; }

        public TestTcpServerHostedService(
            IServiceScopeFactory scopeFactory,
            ILoggerFactory loggerFactory,
            IOptions<TcpServerOptions> options)
            : base(scopeFactory, loggerFactory, options)
        {
        }

        protected override void ApplySocketOptions(TcpClient client)
        {
            ApplySocketOptionsCalled = true;
            base.ApplySocketOptions(client);
        }
    }

    private sealed class TestProtocol : IProtocolHandler
    {
        private readonly TaskCompletionSource _runStarted =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _runCompleted =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        private int _isRunning;

        public bool IsRunning => Volatile.Read(ref _isRunning) == 1;
        public bool IsAttached => IsRunning;
        public ProtocolExceptionHandler? OnExceptionAsync { get; set; }

        public void Disconnect()
        {
            AllowComplete();
        }

        public async Task RunAsync(Stream stream, CancellationToken ct)
        {
            Interlocked.Exchange(ref _isRunning, 1);
            _runStarted.TrySetResult();
            try
            {
                await _runCompleted.Task.WaitAsync(ct);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
            }
        }

        public void AllowComplete() => _runCompleted.TrySetResult();

        public Task WaitForRunAsync(TimeSpan timeout) => WaitWithTimeout(_runStarted.Task, timeout);
        public Task WaitForCompletionAsync(TimeSpan timeout) => WaitWithTimeout(_runCompleted.Task, timeout);

        private static async Task WaitWithTimeout(Task task, TimeSpan timeout)
        {
            if (await Task.WhenAny(task, Task.Delay(timeout)) != task)
            {
                throw new TimeoutException("Task did not complete within the timeout.");
            }
        }
    }
}
