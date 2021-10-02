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
        private static readonly MethodInfo GetServiceMethod = typeof(IServiceProvider)
            .GetMethod(nameof(IServiceProvider.GetService))!;

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
                var activationArgs = new object[descriptor.ActivationParameters.Length];
                activationArgs[0] = next;

                if (activationArgs.Length > 1)
                {
                    Array.Copy(args, 0, activationArgs, 1, args.Length);
                }

                var instance = Activator.CreateInstance(middlewareType, activationArgs)!;

                if (descriptor.InvokeParameters.Length == 1)
                {
                    return (PipelineDelegate<TContext>)descriptor.InvokeMethod.CreateDelegate(
                        typeof(PipelineDelegate<TContext>), 
                        instance);
                }

                var factory = Compile<TContext>(descriptor.InvokeMethod, descriptor.InvokeParameters);

                return (context, services) => factory(instance, context, services);
            });
        }
        
        /// <summary>
        /// Create a wrapping delegate around the implementation's Invoke method
        /// </summary>
        private static Func<object, TContext, IServiceProvider, Task> Compile<TContext>(
            MethodInfo invokeMethod, 
            IReadOnlyList<ParameterInfo> parameters)
        {
            var middleware = Expression.Parameter(typeof(object));
            var context = Expression.Parameter(typeof(TContext));
            var serviceProvider = Expression.Parameter(typeof(IServiceProvider));

            var invokeArgs = new Expression[parameters.Count];

            for (var c = 0; c < parameters.Count; c++)
            {
                var parameter = parameters[c];

                if (parameter.ParameterType == typeof(TContext))
                {
                    invokeArgs[c] = context;
                    continue;
                }

                if (parameter.ParameterType == typeof(IServiceProvider))
                {
                    invokeArgs[c] = serviceProvider;
                    continue;
                }

                var getService = Expression.Call(
                    serviceProvider, 
                    GetServiceMethod,
                    Expression.Constant(parameter.ParameterType));

                invokeArgs[c] = Expression.Convert(getService, parameter.ParameterType);
            }

            var body = Expression.Call(
                Expression.Convert(middleware, invokeMethod.DeclaringType!),
                invokeMethod,
                invokeArgs);

            var lambda = Expression.Lambda<Func<object, TContext, IServiceProvider, Task>>(
                body, middleware, context, serviceProvider);

            return lambda.Compile();
        }
    }
}