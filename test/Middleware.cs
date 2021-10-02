using System;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    public class MiddlewareA
    {
        private readonly PipelineDelegate<TestContext> _next;
        private readonly IDataService _dataService;

        public MiddlewareA(PipelineDelegate<TestContext> next, IDataService dataService)
        {
            _next = next;
            _dataService = dataService;
        }

        public Task InvokeAsync(TestContext context)
        {
            context.Count++;
            
            return _next(context);
        }
    }
    
    public class MiddlewareB
    {
        private readonly PipelineDelegate<TestContext> _next;

        public MiddlewareB(PipelineDelegate<TestContext> next)
        {
            _next = next;
        }

        public Task InvokeAsync(TestContext context, IDataService dataService)
        {
            context.Count++;

            context.Data = dataService.GetData("id");
            
            return _next(context);
        }
    }

    public interface IDataService
    {
        string GetData(string id);
    }
} 