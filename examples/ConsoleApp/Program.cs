using System;
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
                .AddSharedServices()
                .AddLogging(builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information))
                .BuildServiceProvider();

            var pipeline = new PipelineBuilder<AddCustomerRequest>()
                .UseMiddleware<OperationLoggingTask>(services)
                .UseMiddleware<ValidateCustomerTask>(services)
                .UseMiddleware<SaveCustomerRecordTask>(services)
                .UseMiddleware<SendWelcomeEmailTask>(services)
                .Build();

            using var requestScope = services.CreateScope();
            var request = new AddCustomerRequest(requestScope.ServiceProvider)
            {
                Record = new Customer
                {
                    DateModified = DateTime.UtcNow,
                    EmailAddress = "example@vertical-pipelines.com",
                    FirstName = "Testy",
                    LastName = "McTesterson"
                }
            };

            await pipeline(request);

            await Task.Delay(500);

            Console.WriteLine($"Customer {request.NewId} created");
        }
    }
}
