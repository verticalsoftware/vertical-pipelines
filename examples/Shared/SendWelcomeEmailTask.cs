using System.Threading.Tasks;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class SendWelcomeEmailTask
    {
        private readonly PipelineDelegate<AddCustomerRequest> _next;
        private readonly INotificationService _notificationService;

        public SendWelcomeEmailTask(PipelineDelegate<AddCustomerRequest> next,
            INotificationService notificationService)
        {
            _next = next;
            _notificationService = notificationService;
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

            await _next(request);
        }
    }
}