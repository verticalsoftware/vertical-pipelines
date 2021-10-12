using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class SaveCustomerRecordTask
    {
        private readonly PipelineDelegate<AddCustomerRequest> _next;
        private readonly ILogger<SaveCustomerRecordTask> _logger;

        public SaveCustomerRecordTask(
            PipelineDelegate<AddCustomerRequest> next,
            ILogger<SaveCustomerRecordTask> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(AddCustomerRequest request, IRepository repository)
        {
            var assignedId = await repository.SaveCustomerAsync(request.Record);

            request.NewId = assignedId;
            
            _logger.LogInformation("Saved new customer record");

            await _next(request);
        }
    }
}