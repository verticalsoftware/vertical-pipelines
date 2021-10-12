using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vertical.Pipelines.Internal;

namespace Vertical.Pipelines
{
    /// <inheritdoc />
    public class PipelineBuilder<TContext> : IPipelineBuilder<TContext>
    {
        private readonly List<Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>>> _components =
            new List<Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>>>();

        /// <inheritdoc />
        public IPipelineBuilder<TContext> Use(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware)
        {
            _components.Add(middleware ?? throw new ArgumentNullException(nameof(middleware)));
            return this;
        }

        /// <inheritdoc />
        public IPipelineBuilder<TContext> UseMiddleware(Type type, params object?[] args) =>
            AddMiddlewareType(type, args);

        /// <inheritdoc />
        public IPipelineBuilder<TContext> UseMiddleware<T>(params object?[] args) =>
            AddMiddlewareType(typeof(T), args);

        /// <inheritdoc />
        public PipelineDelegate<TContext> Build()
        {
            PipelineDelegate<TContext> pipeline = (context) => Task.CompletedTask;

            for (var c = _components.Count - 1; c >= 0; c--)
            {
                pipeline = _components[c](pipeline);
            }

            return pipeline;
        }
        
        private IPipelineBuilder<TContext> AddMiddlewareType(Type type, object?[] args)
        {
            return Use(next =>
            {
                var descriptor = MiddlewareDescriptor<TContext>.ForType(type);
                var instance = descriptor.CreateInstance(next, args);

                return context => descriptor.CompileHandler()(instance, context);
            });
        }
    }
}