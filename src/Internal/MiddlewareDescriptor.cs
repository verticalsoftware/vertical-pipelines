using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Vertical.Pipelines.Internal
{
    /// <summary>
    /// Maintains metadata about a middleware component.
    /// </summary>
    internal sealed class MiddlewareDescriptor<TContext>
    {
        private readonly Type _implementationType;
        private readonly ConstructorInfo _constructor;
        private readonly object?[] _activationArgs;
        private readonly MethodInfo _invokeMethod;

        private static readonly MethodInfo GetServiceMethod = typeof(IServiceProvider)
            .GetMethod(nameof(IServiceProvider.GetService))!;
        
        private const BindingFlags MethodBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private const string InvokeMethodName = "Invoke";

        internal MiddlewareDescriptor(
            Type implementationType,
            object?[] activationArgs,
            ConstructorInfo constructor,
            MethodInfo invokeMethod)
        {
            _implementationType = implementationType;
            _constructor = constructor;
            _activationArgs = activationArgs;
            _invokeMethod = invokeMethod;
        }

        private const string InvokeAsyncMethodName = "InvokeAsync";

        internal object CreateInstance(PipelineDelegate<TContext> next)
        {
            var constructorArgs = new object[_constructor.GetParameters().Length];
            constructorArgs[0] = next;

            if (constructorArgs.Length > 1)
            {
                Array.Copy(_activationArgs, 0, constructorArgs, 1, _activationArgs.Length);
            }

            return Activator.CreateInstance(_implementationType, constructorArgs)!;
        }

        internal Func<object, TContext, Task> CompileHandler()
        {
            var middleware = Expression.Parameter(typeof(object), "instance");
            var context = Expression.Parameter(typeof(TContext), "context");
            var parameters = _invokeMethod.GetParameters();
            var invokeArgs = new Expression[parameters.Length];
            var serviceProvider = Expression.Convert(context, typeof(IServiceProvider));

            for (var c = 0; c < parameters.Length; c++)
            {
                var parameter = parameters[c];

                if (parameter.ParameterType == typeof(TContext))
                {
                    invokeArgs[c] = context;
                    continue;
                }

                var getService = Expression.Call(
                    serviceProvider, 
                    GetServiceMethod,
                    Expression.Constant(parameter.ParameterType));

                invokeArgs[c] = Expression.Convert(getService, parameter.ParameterType);
            }

            var body = Expression.Call(
                Expression.Convert(middleware, _implementationType),
                _invokeMethod,
                invokeArgs);

            return Expression
                .Lambda<Func<object, TContext, Task>>(body, middleware, context)
                .Compile();
        } 

        internal static MiddlewareDescriptor<TContext> ForType(Type middlewareType, object?[] args)
        {
            // Find compatible constructor
            var constructors = middlewareType.GetConstructors();

            if (constructors.Length == 0)
            {
                throw Exceptions.NoCompatibleConstructor(middlewareType);
            }

            if (constructors.Length != 1)
            {
                throw Exceptions.MultipleConstructors(middlewareType);
            }

            var constructor = constructors[0];
            var constructorArgs = constructor.GetParameters();

            if (constructorArgs[0].ParameterType != typeof(PipelineDelegate<TContext>))
            {
                throw Exceptions.NoCompatibleConstructor(middlewareType);
            }
            
            // Locate Invoke method
            var handlers = middlewareType
                .GetMethods(MethodBindingFlags)
                .Where(mi => mi.Name == InvokeMethodName || mi.Name == InvokeAsyncMethodName)
                .ToArray();

            if (handlers.Length == 0)
            {
                throw Exceptions.NoInvokeMethod(middlewareType, typeof(TContext));
            }

            if (handlers.Length != 1)
            {
                throw Exceptions.MultipleInvokeMethods(middlewareType);
            }

            var handler = handlers[0];
            var handlerArgs = handler.GetParameters();

            if (handlerArgs.All(arg => arg.ParameterType != typeof(TContext)))
            {
                throw Exceptions.NoInvokeMethod(middlewareType, typeof(TContext));
            }

            var byRefParameter = handlerArgs.FirstOrDefault(arg => arg.ParameterType.IsByRef);

            if (byRefParameter != null)
            {
                throw Exceptions.ByRefParametersNotSupported(middlewareType, byRefParameter);
            }

            if (handlerArgs.Length > 1 && !typeof(IServiceProvider).IsAssignableFrom(typeof(TContext)))
            {
                throw Exceptions.ContextIsNotServiceProvider(middlewareType, typeof(TContext));
            }

            return new MiddlewareDescriptor<TContext>(
                middlewareType,
                args,
                constructor,
                handler);
        }
    }
}