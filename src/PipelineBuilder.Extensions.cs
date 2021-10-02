using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Vertical.Pipelines.Internal;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Extension methods for <see cref="IPipelineBuilder{TContext}"/>
    /// </summary>
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Registers a middleware component that uses an application defined class
        /// implementation.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="args">Arguments to pass to the types constructor during activation.</param>
        /// <typeparam name="TContext">
        /// The type of object that carries the contextual state of the activity
        /// to all of the middleware components in the pipeline. 
        /// </typeparam>
        /// <typeparam name="TService">
        /// The implementation type that contains the middleware implementation.
        /// </typeparam>
        /// <returns>A reference to the builder instance.</returns>
        public static IPipelineBuilder<TContext> UseMiddleware<TContext, TService>(
            this IPipelineBuilder<TContext> builder,
            params object?[] args)
        {
            return builder.UseMiddleware(typeof(TService), args);
        }

        /// <summary>
        /// Registers a middleware component that uses an application defined class
        /// implementation.
        /// </summary>
        /// <param name="builder">Pipeline builder.</param>
        /// <param name="middlewareType">The type of class that implements the middleware behavior.</param>
        /// <param name="args">Arguments to pass to the types constructor during activation.</param>
        /// <typeparam name="TContext">
        /// The type of object that carries the contextual state of the activity
        /// to all of the middleware components in the pipeline. 
        /// </typeparam>
        /// <returns>A reference to the builder instance.</returns>
        public static IPipelineBuilder<TContext> UseMiddleware<TContext>(
            this IPipelineBuilder<TContext> builder,
            Type middlewareType, 
            params object?[] args)
        {
            return builder.Use(next =>
            {
                var descriptor = MiddlewareDescriptor<TContext>.ForType(middlewareType, args);
                var instance = descriptor.CreateInstance(next);

                return context => descriptor.CompileHandler()(instance, context);
            });
        }
    }
}