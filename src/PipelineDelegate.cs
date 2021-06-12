using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Defines the <see cref="InvokeAllAsync{TContext}"/> method which can be used to execute
    /// a pipeline of tasks.
    /// </summary>
    public static class PipelineDelegate
    {
        /// <summary>
        /// Invokes a series of tasks that comprise a pipeline of work.  
        /// </summary>
        /// <param name="pipelineTasks">
        /// An enumerable collection of tasks that comprise the pipeline. The tasks
        /// should be arranged in the order that they will execute by default.
        /// </param>
        /// <param name="context">A stateful object that is available to the tasks.</param>
        /// <param name="cancellationToken">A token that can be observed that indicates whether cancellation
        /// is requested.</param>
        /// <typeparam name="TContext">The context object type.</typeparam>
        /// <returns>A task that represents the future completion of the pipeline work.</returns>
        public static Task InvokeAllAsync<TContext>(
            IEnumerable<IPipelineTask<TContext>> pipelineTasks,
            TContext context,
            CancellationToken cancellationToken = default)
        {
            return new PipelineDelegate<TContext>(pipelineTasks).InvokeAsync(context, cancellationToken);
        }
    }
    
    /// <summary>
    /// Represents an object that exposes a method used to invoke tasks in the pipeline.
    /// </summary>
    /// <typeparam name="TContext">The type of context object shared in the pipeline.</typeparam>
    public readonly struct PipelineDelegate<TContext>
    {
        private readonly Queue<IPipelineTask<TContext>> taskQueue;
        
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="pipelineTasks">An enumerable collection of <see cref="IPipelineTask{TContext}"/>
        /// objects that will be executed within the pipeline.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PipelineDelegate(IEnumerable<IPipelineTask<TContext>> pipelineTasks)
        {
            taskQueue = new Queue<IPipelineTask<TContext>>(
                pipelineTasks
                ??
                throw new ArgumentNullException(nameof(pipelineTasks)));
        }

        /// <summary>
        /// Invokes the next task within the pipeline.
        /// </summary>
        /// <param name="context">The client defined context object that is available to tasks
        /// in the pipeline.</param>
        /// <param name="cancellationToken">A token that can be observed that indicates whether cancellation
        /// is requested.</param>
        /// <returns>A Task that represents future completion of the work.</returns>
        public Task InvokeAsync(TContext context, CancellationToken cancellationToken)
        {
            return taskQueue.TryDequeue(out var pipelineTask)
                ? pipelineTask.InvokeAsync(context, this, cancellationToken)
                : Task.CompletedTask;
        }

        /// <inheritdoc />
        public override string ToString() => $"{taskQueue.Count} pending task(s)";
    }
}