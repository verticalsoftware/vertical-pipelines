using System;
using System.Collections.Generic;
using System.Linq;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Creates delegates that can be used to invoke a task pipeline.
    /// </summary>
    public static class PipelineFactory
    {
        /// <summary>
        /// Creates a delegate that routes control and context to a composed set
        /// of middleware tasks.
        /// </summary>
        /// <param name="middlewareDescriptors">
        ///     A collection of <see cref="MiddlewareDescriptor{TContext,TResult}"/> objects
        ///     that define the metadata of each task implementation. The order in which the
        ///     enumerable produces each descriptor is the order in which the task pipeline
        ///     will be composed.
        /// </param>
        /// <param name="serviceProvider">
        ///     The object that will provide dependencies to the middleware implementations
        ///     when they are created. For most dependency injection systems, this should be
        ///     the root provider since the middleware implementations are only created once
        ///     per application lifetime.
        /// </param>
        /// <typeparam name="TContext">
        ///     The type of application provided contextual object that is available to the
        ///     middleware implementations during the orchestration of a pipeline execution.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of value that is returned by the pipeline.
        /// </typeparam>
        /// <returns>
        ///     A stateful delegate that invokes the first middleware component.
        /// </returns>
        public static Func<TContext, IServiceProvider, TResult> Create<TContext, TResult>(
            IEnumerable<MiddlewareDescriptor<TContext, TResult>> middlewareDescriptors,
            IServiceProvider serviceProvider)
        {
            PipelineDelegate<TContext, TResult> next = _ => default!;
            Func<TContext, IServiceProvider, TResult> headCall = null!;

            foreach (var descriptor in middlewareDescriptors.Reverse())
            {
                // Activate an instance
                var middleware = ReflectionHelper.CreateMiddleware(serviceProvider, descriptor, next);
                
                // Create the delegate to Invoke[Async] method
                headCall = ReflectionHelper.CreateInvoker(descriptor, middleware);
                
                // Setup the call chain
                
            }
        }
    }
}