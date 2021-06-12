using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipelinesExample.Pipeline;
using Vertical.Pipelines;

namespace PipelinesExample.Services
{
    public class CloudStorageClient 
    {
        private readonly ILogger<CloudStorageClient> logger;

        public CloudStorageClient(ILogger<CloudStorageClient> logger)
        {
            this.logger = logger;
        }

        public Task ProvisionAccountAsync(string accountId, 
            CancellationToken cancellationToken)
        {
            // Example service implementation
            
            logger.LogInformation("Provisioned storage account for {AccountId}", accountId);

            return Task.CompletedTask;
        }

        public Task DeleteAccountAsync(string accountId,
            CancellationToken cancellationToken)
        {
            // Example service implementation
            
            logger.LogInformation("Deleted storage account for {AccountId}", accountId);

            return Task.CompletedTask;
        }
    }
}