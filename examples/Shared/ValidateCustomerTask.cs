using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class ValidateCustomerTask : IPipelineMiddleware<AddCustomerRequest>
    {
        private readonly ILogger<ValidateCustomerTask> _logger;

        public ValidateCustomerTask(ILogger<ValidateCustomerTask> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public Task InvokeAsync(AddCustomerRequest context, 
            PipelineDelegate<AddCustomerRequest> next, 
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(context.Record.FirstName))
            {
                throw new ApplicationException("First name required.");
            }
            if (string.IsNullOrWhiteSpace(context.Record.LastName))
            {
                throw new ApplicationException("Last name required.");
            }

            _logger.LogInformation("Request validation successful");

            return next(context, cancellationToken);
        }
    }
}