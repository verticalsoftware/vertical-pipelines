using System.Threading;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents the interface of an object that encapsulates the logic
    /// of a task within a pipeline.
    /// </summary>
    /// <typeparam name="TContext">The type of context object shared in the pipeline.</typeparam>
    public interface IPipelineTask<TContext>
    {
        /// <summary>
        /// Handles the logic of the task.
        /// </summary>
        /// <param name="context">A client defined stateful object available to all tasks.</param>
        /// <param name="next">An object that serves as a delegate to the next task in the pipeline.</param>
        /// <param name="cancellationToken">A token that can be observed that indicates a cancellation request.</param>
        /// <returns>A task that represents the future completion of work.</returns>
        Task InvokeAsync(TContext context,
            PipelineDelegate<TContext> next,
            CancellationToken cancellationToken);
    }
}