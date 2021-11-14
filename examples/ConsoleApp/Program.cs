using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.Examples.Shared;
using Vertical.Pipelines;

namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                // Services
                .AddSharedServices()
                .AddLogging(builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information))

                // Middleware pipeline services
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, OperationLoggingTask>()
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, ValidateCustomerTask>()
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, SaveCustomerRecordTask>()
                .AddSingleton<IPipelineMiddleware<AddCustomerRequest>, SendWelcomeEmailTask>()
                .AddSingleton<IPipelineFactory<AddCustomerRequest>, PipelineFactory<AddCustomerRequest>>()
                .AddSingleton(sp => sp.GetRequiredService<IPipelineFactory<AddCustomerRequest>>().CreatePipeline())
                .BuildServiceProvider();

            using var requestScope = services.CreateScope();
            
            var request = new AddCustomerRequest
            {
                Record = new Customer
                {
                    DateModified = DateTime.UtcNow,
                    EmailAddress = "example@vertical-pipelines.com",
                    FirstName = "Testy",
                    LastName = "McTesterson"
                }
            };

            var pipeline = requestScope.ServiceProvider.GetRequiredService<PipelineDelegate<AddCustomerRequest>>();
            
            await pipeline(request, CancellationToken.None);

            await Task.Delay(500);

            Console.WriteLine($"Customer {request.NewId} created");
        }
    }
}
