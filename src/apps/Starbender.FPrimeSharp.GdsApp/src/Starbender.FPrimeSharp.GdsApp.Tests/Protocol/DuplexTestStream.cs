using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.GdsApp.Protocol;

/// <summary>
/// Two connected in-memory streams (A <-> B) built on System.IO.Pipelines.
/// Useful for unit testing protocol handlers without sockets.
/// </summary>
internal static class PipeDuplexStream
{
    public static (Stream A, Stream B) CreatePair()
    {
        var aToB = new Pipe();
        var bToA = new Pipe();

        var a = new PipeBackedStream(readFrom: bToA.Reader, writeTo: aToB.Writer);
        var b = new PipeBackedStream(readFrom: aToB.Reader, writeTo: bToA.Writer);

        return (a, b);
    }

    private sealed class PipeBackedStream : Stream
    {
        private readonly PipeReader _reader;
        private readonly PipeWriter _writer;

        public PipeBackedStream(PipeReader readFrom, PipeWriter writeTo)
        {
            _reader = readFrom;
            _writer = writeTo;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush() => _writer.FlushAsync(CancellationToken.None).GetAwaiter().GetResult();

        public override Task FlushAsync(CancellationToken cancellationToken)
            => _writer.FlushAsync(cancellationToken).AsTask();

        public override int Read(byte[] buffer, int offset, int count)
            => ReadAsync(buffer.AsMemory(offset, count), CancellationToken.None).GetAwaiter().GetResult();

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var result = await _reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var readable = result.Buffer;

                if (!readable.IsEmpty)
                {
                    var toCopy = (int)Math.Min(buffer.Length, readable.Length);
                    readable.Slice(0, toCopy).CopyTo(buffer.Span);

                    _reader.AdvanceTo(readable.GetPosition(toCopy));
                    return toCopy;
                }

                _reader.AdvanceTo(readable.Start, readable.End);

                if (result.IsCompleted)
                    return 0;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
            => WriteAsync(buffer.AsMemory(offset, count), CancellationToken.None).GetAwaiter().GetResult();

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            _writer.Write(buffer.Span);
            var flush = await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            if (flush.IsCanceled) cancellationToken.ThrowIfCancellationRequested();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { _reader.Complete(); } catch { }

                try { _writer.Complete(); } catch { }
            }

            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            try { await _reader.CompleteAsync().ConfigureAwait(false); } catch { }

            try { await _writer.CompleteAsync().ConfigureAwait(false); } catch { }

            await base.DisposeAsync().ConfigureAwait(false);
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
