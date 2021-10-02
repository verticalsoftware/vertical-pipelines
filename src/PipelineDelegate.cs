using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents a function that implements the logic of a middleware component
    /// in a pipeline activity.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of object that carries the contextual state of the activity
    /// through all of the middleware components in the pipeline. 
    /// </typeparam>
    public delegate Task PipelineDelegate<in TContext>(TContext context);
}