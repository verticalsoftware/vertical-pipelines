using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Vertical.Pipelines.Internal
{
    internal static class Exceptions
    {
        internal static Exception MultipleInvokeMethods(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' defines multiple Invoke/InvokeAsync methods which is not supported");
        }

        internal static Exception NoInvokeMethod(Type middlewareType, Type contextType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' does not define a compatible Invoke/InvokeAsync method."
                + $"Compatible method must accept a parameter for '{contextType}' and IServiceProvider.");
        }

        internal static Exception InvokeMethodWrongReturnType(Type middlewareType, Type returnType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' Invoke/InvokeAsync method returns "
                + $"{returnType} instead of {typeof(Task)}");
        }

        internal static Exception InvokeMethodHasNoContextParameter(MethodInfo invokeMethod, Type contextType)
        {
            return new InvalidOperationException(
                $"Middleware method '{invokeMethod}' is missing context parameter for type '{contextType}'");
        }

        internal static Exception NoCompatibleConstructor(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' does not define a compatible internal constructor that "
                + "accepts the next pipeline delegate.");
        }

        internal static Exception MultipleConstructors(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' defines multiple ambiguous constructors (reduce to one).");
        }

        internal static Exception ByRefParametersNotSupported(Type middlewareType, ParameterInfo byRefParameter)
        {
            throw new InvalidOperationException(
                $"Middleware type '{middlewareType}' Invoke/InvokeAsync method parameter "
                + $"for type {byRefParameter.ParameterType}' by ref is not supported.");
        }

        public static Exception ContextIsNotServiceProvider(Type middlewareType, Type contextType)
        {
            throw new InvalidOperationException(
                $"Middleware type '{middlewareType}' Invoke/InvokeAsync method requires dependencies, "
                + $"but context type '{contextType}' does not implement IServiceProvider.");
        }
    }
}