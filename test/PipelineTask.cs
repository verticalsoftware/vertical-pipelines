using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.Pipelines.Test
{
    public class PipelineTask<TContext> : IPipelineTask<TContext>
    {
        private readonly Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> handler;

        public PipelineTask(Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> handler)
        {
            this.handler = handler;
        }
        /// <inheritdoc />
        public Task InvokeAsync(TContext context, PipelineDelegate<TContext> next, CancellationToken cancellationToken)
        {
            return handler(context, next, cancellationToken);
        }
    }
}