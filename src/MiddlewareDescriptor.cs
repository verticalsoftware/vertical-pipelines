using System;
using System.Linq;
using System.Reflection;

namespace Vertical.Pipelines
{
    /// <summary>
    ///     Represents the metadata of a middleware component.
    /// </summary>
    /// <typeparam name="TContext">
    ///     The type of application provided contextual object that is available to the
    ///     middleware implementations during the orchestration of a pipeline execution.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of value that is returned by the pipeline.
    /// </typeparam>
    public class MiddlewareDescriptor<TContext, TResult>
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="type">The middleware component type.</param>
        public MiddlewareDescriptor(Type type)
        {
            Constructor = ResolveConstructor(type);
            InvokeMethod = ResolveInvokeMethod(type);
        }

        /// <summary>
        /// Gets the compatible constructor for the middleware.
        /// </summary>
        public ConstructorInfo Constructor { get; }
        
        /// <summary>
        /// Gets the compatible invoke method for the middleware.
        /// </summary>
        public MethodInfo InvokeMethod { get; }

        private static ConstructorInfo ResolveConstructor(Type type)
        {
            var pipelineDelegateType = typeof(PipelineDelegate<TContext, TResult>);
            var constructors = type
                .GetConstructors()
                .Where(ctor => ctor
                    .GetParameters()
                    .Any(pi => pi.ParameterType == pipelineDelegateType))
                .ToArray();

            switch (constructors.Length)
            {
                case 1:
                    break;
                
                case 0:
                    throw new ArgumentException(
                        $"Middleware type '{type}' does not define a public compatible constructor." 
                        + Environment.NewLine 
                        + "An example would be:" 
                        + Environment.NewLine 
                        + $"\tpublic {type.Name}(PipelineDelegate<TContext, TResult> next) {{ }}");
                
                default:
                    var constructorDescriptions = string.Join(
                        Environment.NewLine,
                        constructors.Select(ctor => ctor.ToString()));
                    throw new ArgumentException(
                        $"Middleware type '{type}' defines multiple constructors which is not supported:"
                        + Environment.NewLine
                        + constructorDescriptions);
            }

            return constructors[0];
        }
        
        private static MethodInfo ResolveInvokeMethod(Type type)
        {
            var invokeMethods = type
                .GetMethods()
                .Where(mi =>
                    (mi.Name == "Invoke" || mi.Name == "InvokeAsync")
                    && mi.ReturnType == typeof(TResult)
                    && mi.GetParameters().Any(pi => pi.ParameterType == typeof(TContext)))
                .ToArray();

            switch (invokeMethods.Length)
            {
                case 1:
                    break;
                
                case 0:
                    throw new ArgumentException(
                        $"Middleware type '{type}' does not define a an 'Invoke' or 'InvokeAsync' method." 
                        + Environment.NewLine 
                        + "An example would be:" 
                        + Environment.NewLine 
                        + $"\tpublic async Task InvokeAsync(RequestContext context) {{ }}");
                
                default:
                    var methodDescriptions = string.Join(
                        Environment.NewLine,
                        invokeMethods.Select(ctor => ctor.ToString()));
                    throw new ArgumentException(
                        $"Middleware type '{type}' defines multiple Invoke/InvokeAsync methods which is not supported:"
                        + Environment.NewLine
                        + methodDescriptions);
            }

            return invokeMethods[0];
        }

        /// <inheritdoc />
        public override string ToString() => Constructor.DeclaringType!.ToString();
    }
}