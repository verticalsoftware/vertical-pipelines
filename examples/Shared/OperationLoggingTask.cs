using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class OperationLoggingTask : IPipelineMiddleware<AddCustomerRequest>
    {
        private readonly ILogger<OperationLoggingTask> _logger;

        public OperationLoggingTask(ILogger<OperationLoggingTask> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(AddCustomerRequest context, 
            PipelineDelegate<AddCustomerRequest> next, 
            CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred processing the add request");
            }
        }
    }
}