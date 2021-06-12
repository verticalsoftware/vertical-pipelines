using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipelinesExample.Models;
using PipelinesExample.Pipeline;
using Vertical.Pipelines;

namespace PipelinesExample.Services
{
    public class UserRepository : IPipelineTask<CreateUserContext>
    {
        private readonly ILogger<UserRepository> logger;

        public UserRepository(ILogger<UserRepository> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(CreateUserContext context, 
            PipelineDelegate<CreateUserContext> next, 
            CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            
            // "Save" user record
            logger.LogInformation("Saved user record, generated id = {UserId}", id);

            context.EntityId = id;

            await next.InvokeAsync(context, cancellationToken);
        }
    }
}