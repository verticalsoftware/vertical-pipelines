using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    public class MiddlewareA
    {
        private readonly PipelineDelegate<TestContext, Task> _next;

        public MiddlewareA(PipelineDelegate<TestContext, Task> next)
        {
            _next = next;
        }

        public async Task InvokeAsync(TestContext context)
        {
            context.Count++;

            await _next(context);
        }   
    }

    public class MiddlewareB
    {
        private readonly PipelineDelegate<TestContext, Task> _next;

        public MiddlewareB(PipelineDelegate<TestContext, Task> next)
        {
            _next = next;
        }

        public async Task InvokeAsync(TestContext context)
        {
            context.Count++;

            await _next(context);
        }  
    }
    
    public class MiddlewareC
    {
        private readonly PipelineDelegate<TestContext, Task> _next;

        public MiddlewareC(PipelineDelegate<TestContext, Task> next)
        {
            _next = next;
        }

        public async Task InvokeAsync(TestContext context)
        {
            context.Count++;

            await _next(context);
        }  
    }
} 