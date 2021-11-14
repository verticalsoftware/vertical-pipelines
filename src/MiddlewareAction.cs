using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents an implementation of <see cref="IPipelineMiddleware{TContext}"/>
    /// that wraps a handling delegate.
    /// </summary>
    /// <typeparam name="TContext">Contextual data type of the pipeline.</typeparam>
    public class MiddlewareAction<TContext> : IPipelineMiddleware<TContext>
        where TContext : class
    {
        private readonly Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> _implementation;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="implementation">Delegate that handles the middleware implementation.</param>
        public MiddlewareAction(Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> implementation)
        {
            _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(TContext context, 
            PipelineDelegate<TContext> next, 
            CancellationToken cancellationToken)
        {
            return _implementation(context, next, cancellationToken);
        }
    }
}