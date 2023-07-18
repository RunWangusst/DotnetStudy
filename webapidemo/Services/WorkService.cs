namespace webapidemo.Services
{
    public class WorkService : IWorkService
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private int executionCount = 0;

        public WorkService(ILogger<WorkService> logger)
        {
            _logger = logger;
        }
        public Task DoWork()
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation($"Service processing {count}");

            return Task.CompletedTask;
        }
    }
}
