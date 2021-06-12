using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipelinesExample.Models;
using PipelinesExample.Pipeline;
using PipelinesExample.Services;
using Vertical.Pipelines;

namespace PipelinesExample
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            
            // Register infrastructure services
            services.AddScoped<UserRepository>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<CloudStorageClient>();
            services.AddLogging(logging => logging.AddConsole());
            
            // Construct the pipeline here. Place tasks in the order
            // they should be evaluated. The registration scope is
            // entirely up to you, but the best practice may be scoped
            // services
            services.AddScoped<IPipelineTask<CreateUserContext>, SendWelcomeEmail>();
            services.AddScoped<IPipelineTask<CreateUserContext>, SaveUserRecord>();
            services.AddScoped<IPipelineTask<CreateUserContext>, ProvisionStorageAccount>();

            await using var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();
            
            // Get the tasks from DI
            var pipelineTasks = scope.ServiceProvider.GetRequiredService<IEnumerable<IPipelineTask<CreateUserContext>>>();

            await PipelineDelegate.InvokeAllAsync(
                pipelineTasks,
                new CreateUserContext
                {
                    Model = new UserModel(
                        FirstName: "Testy",
                        LastName: "McTesterson",
                        EmailAddress: "testy@pipelines.com")
                });
        }
    }
}
