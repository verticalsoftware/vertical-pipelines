using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.Examples.Shared;
using Vertical.Pipelines;

namespace Vertical.Examples.WebApp
{
    public static class PipelineConfiguration
    {
        public static void ConfigureCustomPipeline(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var pipeline = new PipelineBuilder<AddCustomerRequest>()
                    .UseMiddleware<OperationLoggingTask>(provider)
                    .UseMiddleware<ValidateCustomerTask>(provider)
                    .UseMiddleware<SaveCustomerRecordTask>(provider)
                    .UseMiddleware<SendWelcomeEmailTask>(provider)
                    .Build();

                return pipeline;
            });
        }
    }
}