using System;

namespace Vertical.Pipelines
{
    public class TestContext : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TestContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public int Count { get; set; }
        
        public string Data { get; set; }

        /// <inheritdoc />
        object? IServiceProvider.GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}