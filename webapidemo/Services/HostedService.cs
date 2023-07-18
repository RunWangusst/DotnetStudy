namespace webapidemo.Services
{
    public class HostedService : IHostedService, IDisposable
    {
        private readonly ILogger<HostedService> _logger;
        public IServiceProvider Services { get; private set; }
        private Timer _timer;

        public HostedService(IServiceProvider service, ILogger<HostedService> logger)
        {
            Services = service;
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        private void DoWork(object  state)
        {
            _logger.LogInformation("Service working");

            using (var scope= Services.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IWorkService>();

                scopedProcessingService.DoWork().GetAwaiter().GetResult();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting Service {cancellationToken}");

            _timer = new Timer(DoWork,null,TimeSpan.Zero,TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stop Service {cancellationToken}");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
