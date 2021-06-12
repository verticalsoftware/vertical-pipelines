using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipelinesExample.Pipeline;
using Vertical.Pipelines;

namespace PipelinesExample.Services
{
    public class EmailService : IPipelineTask<CreateUserContext>
    {
        private readonly ILogger<EmailService> logger;

        public EmailService(ILogger<EmailService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(CreateUserContext context, 
            PipelineDelegate<CreateUserContext> next, 
            CancellationToken cancellationToken)
        {
            // Here we'll defer sending out the email until we know
            // everything else is complete
            await next.InvokeAsync(context, cancellationToken);
            
            // "Send" the email out
            // smtp.SendEmail(...)
        }
    }
}