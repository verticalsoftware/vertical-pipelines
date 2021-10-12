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
        private readonly MethodInfo _invokeMethod;

        private const BindingFlags MethodBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private const string InvokeMethodName = "Invoke";

        internal MiddlewareDescriptor(
            Type implementationType,
            ConstructorInfo constructor,
            MethodInfo invokeMethod)
        {
            _implementationType = implementationType;
            _constructor = constructor;
            _invokeMethod = invokeMethod;
        }

        private const string InvokeAsyncMethodName = "InvokeAsync";

        internal object CreateInstance(PipelineDelegate<TContext> next, object?[] args)
        {
            var parameters = _constructor.GetParameters();
            var constructorArgs = new object[parameters.Length];
            constructorArgs[0] = next;

            switch (args.Length)
            {
                case 0:
                    break;
                
                case 1 when args[0] is IServiceProvider serviceProvider:
                    // Map parameter values using service provider
                    for (var c = 1; c < parameters.Length; c++)
                    {
                        constructorArgs[c] = serviceProvider.GetService(parameters[c].ParameterType)!;
                    }
                    break;
                
                default:
                    Array.Copy(args, 0, constructorArgs, 1, args.Length);
                    break;
            }
            
            return _constructor.Invoke(constructorArgs);
        }

        internal Func<object, TContext, Task> CompileHandler()
        {
            var middleware = Expression.Parameter(typeof(object), "instance");
            var context = Expression.Parameter(typeof(TContext), "context");
            var parameters = _invokeMethod.GetParameters();
            var invokeArgs = new Expression[parameters.Length];
            var getServiceMetadata = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))!;
            Expression body;

            if (parameters.Length == 1)
            {
                invokeArgs[0] = context;
                
                body = Expression.Call(
                    Expression.Convert(middleware, _implementationType),
                    _invokeMethod,
                    invokeArgs);

                return Expression
                    .Lambda<Func<object, TContext, Task>>(body, middleware, context)
                    .Compile();
            }

            var appServicesImpl = Expression.Convert(context, typeof(IApplicationServices));
            var servicesAccessor = Expression.Property(appServicesImpl, nameof(IApplicationServices.ApplicationServices));
            var serviceProvider = Expression.Variable(typeof(IServiceProvider));

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
                    getServiceMetadata,
                    Expression.Constant(parameter.ParameterType));

                invokeArgs[c] = Expression.Convert(getService, parameter.ParameterType);
            }
                
            body = Expression.Call(
                Expression.Convert(middleware, _implementationType),
                _invokeMethod,
                invokeArgs);

            var block = Expression.Block(
                typeof(Task),
                new[] { serviceProvider }, 
                Expression.Assign(serviceProvider, servicesAccessor), 
                body);

            return Expression
                .Lambda<Func<object, TContext, Task>>(block, middleware, context)
                .Compile();
        } 

        internal static MiddlewareDescriptor<TContext> ForType(Type middlewareType)
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

            if (handler.ReturnType != typeof(Task))
            {
                throw Exceptions.InvokeMethodWrongReturnType(middlewareType, handler.ReturnType);
            }
            
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

            if (handlerArgs.Length > 1 && !typeof(IApplicationServices).IsAssignableFrom(typeof(TContext)))
            {
                throw Exceptions.ContextIsNotServiceProvider(middlewareType, typeof(TContext));
            }

            return new MiddlewareDescriptor<TContext>(
                middlewareType,
                constructor,
                handler);
        }
    }
}