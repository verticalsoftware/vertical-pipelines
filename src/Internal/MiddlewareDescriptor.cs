using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vertical.Pipelines.Internal
{
    /// <summary>
    /// Maintains metadata about a middleware component.
    /// </summary>
    internal sealed class MiddlewareDescriptor<TContext>
    {

        private const BindingFlags MethodBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private const string InvokeMethodName = "Invoke";
        private const string InvokeAsyncMethodName = "InvokeAsync";

        internal MiddlewareDescriptor(
            Type implementationType,
            ConstructorInfo constructor,
            ParameterInfo[] activationParameters,
            object?[] activationArgs,
            MethodInfo invokeMethod,
            ParameterInfo[] invokeParameters)
        {
            ImplementationType = implementationType;
            Constructor = constructor;
            ActivationParameters = activationParameters;
            ActivationArgs = activationArgs;
            InvokeMethod = invokeMethod;
            InvokeParameters = invokeParameters;
        }
        
        internal Type ImplementationType { get; }
        
        internal ConstructorInfo Constructor { get; }
        
        internal ParameterInfo[] ActivationParameters { get; }
        
        internal object?[] ActivationArgs { get; }

        internal MethodInfo InvokeMethod { get; }
        
        internal ParameterInfo[] InvokeParameters { get; }

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
            var typeHash = new HashSet<Type>(handlerArgs.Select(arg => arg.ParameterType));

            if (!(typeHash.Contains(typeof(TContext)) && typeHash.Contains(typeof(IServiceProvider))))
            {
                throw Exceptions.NoInvokeMethod(middlewareType, typeof(TContext));
            }

            var byRefParameter = handlerArgs.FirstOrDefault(arg => arg.ParameterType.IsByRef);

            if (byRefParameter != null)
            {
                throw Exceptions.ByRefParametersNotSupported(middlewareType, byRefParameter);
            }

            return new MiddlewareDescriptor<TContext>(
                middlewareType,
                constructor,
                constructorArgs,
                args,
                handler,
                handlerArgs);
        }
    }
}