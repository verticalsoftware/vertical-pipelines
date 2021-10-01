using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    internal static class Exceptions
    {
        public static Exception MultipleInvokeMethods(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' defines multiple Invoke/InvokeAsync methods which is not supported");
        }

        public static Exception NoInvokeMethod(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' does not define an Invoke/InvokeAsync method");
        }

        public static Exception InvokeMethodWrongReturnType(Type middlewareType, Type returnType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' Invoke/InvokeAsync method returns "
                + $"{returnType} instead of {typeof(Task)}");
        }

        public static Exception InvokeMethodHasNoContextParameter(MethodInfo invokeMethod, Type contextType)
        {
            return new InvalidOperationException(
                $"Middleware method '{invokeMethod}' is missing context parameter for type '{contextType}'");
        }

        public static Exception NoCompatibleConstructor(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' does not define a compatible public constructor.");
        }

        public static Exception MultipleConstructors(Type middlewareType)
        {
            return new InvalidOperationException(
                $"Middleware type '{middlewareType}' defines multiple ambiguous constructors (reduce to one).");
        }
    }
}