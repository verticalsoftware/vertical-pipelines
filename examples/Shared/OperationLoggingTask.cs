using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class OperationLoggingTask
    {
        private readonly PipelineDelegate<AddCustomerRequest> _next;
        private readonly ILogger<OperationLoggingTask> _logger;

        public OperationLoggingTask(PipelineDelegate<AddCustomerRequest> next,
            ILogger<OperationLoggingTask> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(AddCustomerRequest request)
        {
            try
            {
                await _next(request);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred processing the add request");
            }
        }
    }
}