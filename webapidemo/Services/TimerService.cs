using Microsoft.AspNetCore.Mvc;

namespace webapidemo.Services
{
    public class TimerService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private int executionCount = 0;

        private Timer _timer;

        public TimerService(ILogger<TimerService> logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation($"Service processing {count}");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(TimerService)} started");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stop");

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
