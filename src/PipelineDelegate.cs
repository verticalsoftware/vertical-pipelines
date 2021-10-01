using System;
using System.Threading.Tasks;

namespace Vertical.Pipelines
{
    public delegate Task PipelineDelegate<in TContext>(TContext context, IServiceProvider serviceProvider);
}