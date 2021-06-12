using System.Threading;
using System.Threading.Tasks;
using PipelinesExample.Services;
using Vertical.Pipelines;

namespace PipelinesExample.Pipeline
{
    public class SendWelcomeEmail : IPipelineTask<CreateUserContext>
    {
        private readonly EmailService emailService;

        public SendWelcomeEmail(EmailService emailService)
        {
            this.emailService = emailService;
        }
        
        /// <inheritdoc />
        public async Task InvokeAsync(CreateUserContext context, 
            PipelineDelegate<CreateUserContext> next, 
            CancellationToken cancellationToken)
        {
            // Only send the email at the end if all other tasks
            // complete successfully
            await next.InvokeAsync(context, cancellationToken);

            await emailService.SendMessageAsync(
                "example@vertical-software.com",
                context.Model.EmailAddress,
                "Welcome!",
                "Your account has been created.",
                cancellationToken);
  ;      }
    }
}