using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Website.Repositories;

namespace Website.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IAccountDumpRepository _repository;

        public CleanupService(IAccountDumpRepository repository)
        {
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                _repository.Cleanup();
            }
        }
    }
}
