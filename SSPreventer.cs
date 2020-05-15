using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SSPreventer
{
    public class SSPreventer : IHostedService
    {
        private sealed class Execution : IAsyncDisposable
        {
            public static Execution CompletedExecution { get; } = new Execution();

            private readonly CancellationTokenSource cancellation;

            private readonly Task execution;

            private bool isDisposed;

#nullable disable warnings
            private Execution()
            {
                this.isDisposed = true;
                GC.SuppressFinalize(this);
            }
#nullable restore

            public Execution(Func<CancellationToken, Task> process)
            {
                this.cancellation = new CancellationTokenSource();
                this.execution = process(this.cancellation.Token);
            }

            ~Execution()
                => this.cancellation.Cancel();

            public async ValueTask DisposeAsync()
            {
                if (this.isDisposed)
                    return;

                this.isDisposed = true;

                this.cancellation.Cancel();

                try
                {
                    await this.execution.ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    // expected path
                }

                this.cancellation.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        private readonly ILogger<SSPreventer> _logger;

        private readonly MouseController _controller;

        private readonly CursorPositionGetter _positionGetter;

        private readonly int _interval;

        private Execution currentExecution = Execution.CompletedExecution;

        public SSPreventer(ILogger<SSPreventer> logger, MouseController controller, CursorPositionGetter positionGetter, ISSPreventerConfig config)
        {
            this._logger = logger;
            this._controller = controller;
            this._positionGetter = positionGetter;
            this._interval = config.IntervalSeconds * 1000; // milliseconds
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"{nameof(SSPreventer)} started at {DateTimeOffset.Now}");
            this.currentExecution = new Execution(this.Run);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"{nameof(SSPreventer)} is shutting down at {DateTimeOffset.Now}");

            await this.currentExecution.DisposeAsync().ConfigureAwait(false);
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while (true)
            {
                var current = this._positionGetter.GetCursorPosition();

                await Task.Delay(this._interval, cancellationToken).ConfigureAwait(false);

                if (current == this._positionGetter.GetCursorPosition())
                    this.MoveCursor(current);
            }
        }

        private void MoveCursor(Position current)
        {
            this._logger.LogInformation($"{nameof(SSPreventer)} triggered at {DateTimeOffset.Now}");

            do
            {
                var random = (Span<ushort>)stackalloc ushort[2];
                RandomNumberGenerator.Fill(MemoryMarshal.AsBytes(random));

                this._controller.MoveAbsolute(random[0], random[1]);
            }
            while (current == this._positionGetter.GetCursorPosition());

            this._logger.LogInformation("{from} -> {to}", current, this._positionGetter.GetCursorPosition());
        }
    }
}
