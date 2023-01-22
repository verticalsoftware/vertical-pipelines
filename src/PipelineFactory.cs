using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents the default implementation of a component that composes a callable
    /// pipeline delegate.
    /// </summary>
    /// <typeparam name="TContext">Context type begin passed throughout the pipeline.</typeparam>
    public class PipelineFactory<TContext> : IPipelineFactory<TContext> where TContext : class
    {
        private readonly IPipelineMiddleware<TContext>[] _middleware;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="middleware">
        /// An enumerable collection of ordered middleware components to compose in the pipeline.
        /// </param>
        public PipelineFactory(IEnumerable<IPipelineMiddleware<TContext>> middleware)
        {
            _middleware = middleware.Reverse().ToArray();
        }
        
        /// <inheritdoc />
        public PipelineDelegate<TContext> CreatePipeline()
        {
            var next = new PipelineDelegate<TContext>((_, __) => Task.CompletedTask);

            foreach (var component in _middleware)
            {
                var instanceNext = next;
                
                next = (context, cancellationToken) => component.InvokeAsync(context, instanceNext, cancellationToken);
            }

            return next;
        }

        /// <inheritdoc />
        public int TaskCount => _middleware.Length;
    }
}