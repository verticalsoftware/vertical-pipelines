using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class SendWelcomeEmailTask
    {
        private readonly PipelineDelegate<AddCustomerRequest> _next;
        private readonly INotificationService _notificationService;
        private readonly ILogger<SendWelcomeEmailTask> _logger;

        public SendWelcomeEmailTask(PipelineDelegate<AddCustomerRequest> next,
            INotificationService notificationService,
            ILogger<SendWelcomeEmailTask> logger)
        {
            _next = next;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task InvokeAsync(AddCustomerRequest request)
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

            await _next(request);
        }
    }
}