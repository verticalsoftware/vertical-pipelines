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
            services
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, OperationLoggingTask>()
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, ValidateCustomerTask>()
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, SaveCustomerRecordTask>()
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, SendWelcomeEmailTask>()
                .AddSingleton<IPipelineFactory<AddCustomerRequest>, PipelineFactory<AddCustomerRequest>>()
                .AddSingleton(sp => sp.GetRequiredService<IPipelineFactory<AddCustomerRequest>>().CreatePipeline());
        }
    }
}