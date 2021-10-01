namespace Vertical.Pipelines
{
    /// <summary>
    /// Defines a delegate that is used to route control flow and state
    /// throughout an ordered pipeline of tasks.
    /// </summary>
    /// <typeparam name="TContext">The type of context being shared among pipeline tasks.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the pipeline.</typeparam>
    public delegate TResult PipelineDelegate<in TContext, out TResult>(TContext context);
}