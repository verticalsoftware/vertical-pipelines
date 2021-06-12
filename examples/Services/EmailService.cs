using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipelinesExample.Pipeline;
using Vertical.Pipelines;

namespace PipelinesExample.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> logger;

        public EmailService(ILogger<EmailService> logger)
        {
            this.logger = logger;
        }

        public Task SendMessageAsync(
            string fromAddress,
            string toAddress,
            string subject,
            string body,
            CancellationToken cancellationToken)
        {
            // Example service
            logger.LogInformation("Sent message to {EmailAddress}", toAddress);

            return Task.CompletedTask;
        }
    }
}