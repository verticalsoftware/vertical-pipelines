using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <inheritdoc />
    public class PipelineBuilder<TContext> : IPipelineBuilder<TContext>
    {
        private readonly List<Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>>> _components =
            new List<Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>>>();

        /// <inheritdoc />
        public PipelineBuilder<TContext> Use(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware)
        {
            _components.Add(middleware ?? throw new ArgumentNullException(nameof(middleware)));
            return this;
        }

        /// <inheritdoc />
        public PipelineDelegate<TContext> Build()
        {
            PipelineDelegate<TContext> pipeline = (context, provider) => Task.CompletedTask;

            for (var c = _components.Count - 1; c >= 0; c--)
            {
                pipeline = _components[c](pipeline);
            }

            return pipeline;
        }
    }
}