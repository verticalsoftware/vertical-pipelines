using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipelinesExample.Pipeline;
using Vertical.Pipelines;

namespace PipelinesExample.Services
{
    public class CloudStorageClient : IPipelineTask<CreateUserContext>
    {
        private readonly ILogger<CloudStorageClient> logger;

        public CloudStorageClient(ILogger<CloudStorageClient> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(CreateUserContext context, 
            PipelineDelegate<CreateUserContext> next, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Provisioned storage account for {EmailAddress}", context.Model.EmailAddress);

            await next.InvokeAsync(context, cancellationToken);
        }
    }
}