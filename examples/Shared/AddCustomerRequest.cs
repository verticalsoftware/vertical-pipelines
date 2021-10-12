using System;
using Vertical.Pipelines;

namespace Vertical.Examples.Shared
{
    public class AddCustomerRequest : IApplicationServices
    {
        private readonly IServiceProvider _serviceProvider;

        public AddCustomerRequest(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public Customer Record { get; set; }
        
        public Guid NewId { get; set; }

        /// <inheritdoc />
        IServiceProvider IApplicationServices.ApplicationServices => _serviceProvider;
    }
}