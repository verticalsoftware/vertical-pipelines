using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class ValidateCustomerTask
    {
        private readonly PipelineDelegate<AddCustomerRequest> _next;
        private readonly ILogger<ValidateCustomerTask> _logger;

        public ValidateCustomerTask(
            PipelineDelegate<AddCustomerRequest> next,
            ILogger<ValidateCustomerTask> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(AddCustomerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Record.FirstName))
            {
                throw new ApplicationException("First name required.");
            }
            if (string.IsNullOrWhiteSpace(request.Record.LastName))
            {
                throw new ApplicationException("Last name required.");
            }

            return _next(request);
        }
    }
}