using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Vertical.Examples.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            var repository = Substitute.For<IRepository>();
            repository.SaveCustomerAsync(Arg.Any<Customer>()).Returns(Task.FromResult(Guid.NewGuid()));

            services.AddSingleton(repository);
            services.AddSingleton(Substitute.For<INotificationService>());
            services.AddLogging();

            return services;
        }
    }
}