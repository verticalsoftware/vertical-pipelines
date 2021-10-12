using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vertical.Examples.Shared;
using Vertical.Pipelines;

namespace Vertical.Examples.WebApp
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly PipelineDelegate<AddCustomerRequest> _pipeline;
        private readonly IServiceProvider _serviceProvider;

        public CustomerController(PipelineDelegate<AddCustomerRequest> pipeline, IServiceProvider serviceProvider)
        {
            _pipeline = pipeline;
            _serviceProvider = serviceProvider;
        }
        
        [HttpPost, Route("/api/customers")]
        public async Task<ActionResult> Post([FromBody] AddCustomerRequestModel model)
        {
            var request = new AddCustomerRequest(_serviceProvider)
            {
                Record = model.Record
            };
                
            await _pipeline(request);

            return Created($"/api/customers/{request.NewId}", request);
        }
    }
}