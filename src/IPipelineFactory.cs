namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents a service that composes a pipeline of middleware components
    /// </summary>
    /// <typeparam name="TContext">The contextual data type </typeparam>
    public interface IPipelineFactory<in TContext> where TContext : class
    {
        /// <summary>
        /// When implemented by a class, constructs a callable pipeline delegate.
        /// </summary>
        /// <returns><see cref="PipelineDelegate{TContext}"/></returns>
        PipelineDelegate<TContext> CreatePipeline();
    }
}