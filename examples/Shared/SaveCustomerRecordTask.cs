using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class SaveCustomerRecordTask : IPipelineMiddleware<AddCustomerRequest>
    {
        private readonly ILogger<SaveCustomerRecordTask> _logger;
        private readonly IRepository _repository;

        public SaveCustomerRecordTask(ILogger<SaveCustomerRecordTask> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(AddCustomerRequest context, 
            PipelineDelegate<AddCustomerRequest> next, 
            CancellationToken cancellationToken)
        {
            var assignedId = await _repository.SaveCustomerAsync(context.Record);

            context.NewId = assignedId;
            
            _logger.LogInformation("Saved new customer record");

            await next(context, cancellationToken);
        }
    }
}