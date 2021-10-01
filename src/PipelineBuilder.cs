using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    public class PipelineBuilder<TContext>
    {
        private static BindingFlags InvokeBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private const string InvokeMethodName = "Invoke";
        private const string InvokeAsyncMethodName = "InvokeAsync";
        
        private readonly IServiceProvider _applicationServices;

        private readonly List<Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>>> _components =
            new List<Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>>>();

        public PipelineBuilder(IServiceProvider applicationServices)
        {
            _applicationServices = applicationServices;
        }

        public PipelineBuilder<TContext> Use(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public PipelineBuilder<TContext> UseMiddleware(Type middlewareType, params object?[] args)
        {
            var applicationServices = _applicationServices;

            return Use(next =>
            {
                var invokeMethods = middlewareType
                    .GetMethods(InvokeBindingFlags)
                    .Where(method => method.Name == InvokeMethodName || method.Name == InvokeAsyncMethodName)
                    .ToArray();

                if (invokeMethods.Length > 1)
                {
                    throw Exceptions.MultipleInvokeMethods(middlewareType);
                }

                if (invokeMethods.Length == 0)
                {
                    throw Exceptions.NoInvokeMethod(middlewareType);
                }

                var invokeMethod = invokeMethods[0];

                if (!typeof(Task).IsAssignableFrom(invokeMethod.ReturnType))
                {
                    throw Exceptions.InvokeMethodWrongReturnType(middlewareType, invokeMethod.ReturnType);
                }

                var parameters = invokeMethod.GetParameters();
                var contextParameter = parameters.FirstOrDefault(pi => pi.ParameterType == typeof(TContext) && !pi.ParameterType.IsByRef);

                if (contextParameter == null)
                {
                    throw Exceptions.InvokeMethodHasNoContextParameter(invokeMethod, typeof(TContext));
                }

                var constructors = middlewareType
                    .GetConstructors()
                    .Where(ci =>
                    {
                        var parameterCount = ci.GetParameters().Length;
                        return parameterCount == (args?.Length).GetValueOrDefault() + 1;
                    })
                    .ToArray();

                if (constructors.Length == 0)
                {
                    throw Exceptions.NoCompatibleConstructor(middlewareType);
                }

                if (constructors.Length > 1)
                {
                    throw Exceptions.MultipleConstructors(middlewareType);
                }

                var constructor = constructors[0];
                var activationArgs = new object[constructor.GetParameters().Length + 1];
                activationArgs[0] = next;

                if (activationArgs.Length > 1)
                {
                    Array.Copy(args, 0, activationArgs, 1, args.Length);
                }

                var instance = Activator.CreateInstance(middlewareType, activationArgs)!;

                if (parameters.Length == 1)
                {
                    return (PipelineDelegate<TContext>)invokeMethod.CreateDelegate(typeof(PipelineDelegate<TContext>), instance);
                }

                var factory = CompileInvocation(invokeMethod, parameters);

                return (context, services) => factory(instance, context, services);
            });
        }

        private static Func<object, TContext, IServiceProvider, Task> CompileInvocation(MethodInfo invokeMethod, ParameterInfo[] parameters)
        {
            
        }
    }
}