using System.Threading;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Defines a delegate used to forward program control to another task.
    /// </summary>
    /// <typeparam name="TContext">Context type being passed through the pipeline.</typeparam>
    public delegate Task PipelineDelegate<in TContext>(TContext context, CancellationToken cancellationToken)
        where TContext : class;
}