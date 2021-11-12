using System.Threading;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents an object that handles the implementation of a task in a
    /// pipeline.
    /// </summary>
    /// <typeparam name="TContext">Context type</typeparam>
    public interface IPipelineMiddleware<TContext> where TContext : class
    {
        /// <summary>
        /// When implemented by a class, performs a discrete responsibility and then
        /// optionally forward control flow to the next configured pipeline task.
        /// </summary>
        /// <param name="context">Contextual object data passed throughout the pipeline.</param>
        /// <param name="next">A delegate that passes control to the next configured middleware.</param>
        /// <param name="cancellationToken">A token that can be observed for cancellation requests.</param>
        /// <returns>Task</returns>
        Task InvokeAsync(TContext context, PipelineDelegate<TContext> next, CancellationToken cancellationToken);
    }
}