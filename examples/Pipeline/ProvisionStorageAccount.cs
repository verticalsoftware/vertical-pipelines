using System.Threading;
using System.Threading.Tasks;
using PipelinesExample.Services;
using Vertical.Pipelines;

namespace PipelinesExample.Pipeline
{
    public class ProvisionStorageAccount : IPipelineTask<CreateUserContext>
    {
        private readonly CloudStorageClient storageClient;

        public ProvisionStorageAccount(CloudStorageClient storageClient)
        {
            this.storageClient = storageClient;
        }
        
        /// <inheritdoc />
        public async Task InvokeAsync(CreateUserContext context, 
            PipelineDelegate<CreateUserContext> next, 
            CancellationToken cancellationToken)
        {
            var storageAccountId = $"{context.EntityId}-storage";
            
            await storageClient.ProvisionAccountAsync(storageAccountId,
                cancellationToken);

            try
            {
                await next.InvokeAsync(context, cancellationToken);
            }
            catch
            {
                // Remove storage is error occurs elsewhere in the pipeline
                await storageClient.DeleteAccountAsync(storageAccountId,
                    cancellationToken);
                throw;
            }
        }
    }
}