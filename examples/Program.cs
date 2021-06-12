using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PipelinesExample.Services;

namespace PipelinesExample
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            
            // Register services
            services.AddScoped<UserRepository>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<CloudStorageClient>();

        }
    }
}
