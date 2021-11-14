using System;
using System.Threading;
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

        public CustomerController(PipelineDelegate<AddCustomerRequest> pipeline)
        {
            _pipeline = pipeline;
        }
        
        [HttpPost, Route("/api/customers")]
        public async Task<ActionResult> Post([FromBody] AddCustomerRequestModel model)
        {
            var request = new AddCustomerRequest
            {
                Record = model.Record
            };
                
            await _pipeline(request, CancellationToken.None);

            return Created($"/api/customers/{request.NewId}", request);
        }
    }
}