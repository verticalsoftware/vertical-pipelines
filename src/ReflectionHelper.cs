using System;
using System.Linq;
using System.Linq.Expressions;

namespace Vertical.Pipelines
{
    internal static class ReflectionHelper
    {
        internal static object CreateMiddleware<TContext, TResult>(
            IServiceProvider serviceProvider,
            MiddlewareDescriptor<TContext, TResult> descriptor,
            PipelineDelegate<TContext, TResult> next)
        {
            var constructor = descriptor.Constructor;
            
            var injections = constructor
                .GetParameters()
                .Select(parameter => parameter.ParameterType == typeof(PipelineDelegate<TContext, TResult>)
                    ? next : serviceProvider.GetService(parameter.ParameterType))
                .ToArray();

            return constructor.Invoke(injections);
        }

        internal static Func<TContext, IServiceProvider, TResult> CreateInvoker<TContext, TResult>(
            MiddlewareDescriptor<TContext, TResult> descriptor,
            object instance)
        {
            var method = descriptor.InvokeMethod;
            var parameters = method.GetParameters();

            // Parameters
            var middleware = Expression.Parameter(typeof(object));
            var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
            var getService = Expression.Call(
                serviceProvider,
                typeof(IServiceProvider).GetMethod("GetService")!);
            var context = Expression.Parameter(typeof(TContext));
            
            // Map parameter values
            var invokeParameterValues = parameters
                .Select(parameter => parameter.ParameterType == typeof(TContext)
                    ? (Expression)context
                    : getService)
                .ToArray();

            var invoke = Expression.Call(
                Expression.Convert(middleware, instance.GetType()),
                method,
                invokeParameterValues);

            var lambda = Expression.Lambda<Func<object, TContext, IServiceProvider, TResult>>(
                invoke,
                middleware,
                context,
                serviceProvider);

            var invocationWrapper = lambda.Compile();

            return (ctx, provider) => invocationWrapper(instance, ctx, provider);
        }
    }
}