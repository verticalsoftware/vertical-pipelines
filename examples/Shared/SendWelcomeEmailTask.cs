using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class SendWelcomeEmailTask : IPipelineMiddleware<AddCustomerRequest>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<SendWelcomeEmailTask> _logger;

        public SendWelcomeEmailTask(INotificationService notificationService,
            ILogger<SendWelcomeEmailTask> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(AddCustomerRequest request, 
            PipelineDelegate<AddCustomerRequest> next, 
            CancellationToken cancellationToken)
        {
            await _notificationService.SendEmailAsync(
                "admin@vertical-example.com",
                request.Record.EmailAddress,
                new
                {
                    Subject = "Welcome to Vertical Software",
                    Body = $"Welcome {request.Record.FirstName}, your account has been activated successfully"
                });
            
            _logger.LogInformation("Welcome email sent");

            await next(request, cancellationToken);
        }
    }
}