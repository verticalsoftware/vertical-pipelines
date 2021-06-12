using System.Threading;
using System.Threading.Tasks;
using PipelinesExample.Services;
using Vertical.Pipelines;

namespace PipelinesExample.Pipeline
{
    public class SaveUserRecord : IPipelineTask<CreateUserContext>
    {
        private readonly UserRepository repository;

        public SaveUserRecord(UserRepository repository)
        {
            this.repository = repository;
        }
        
        /// <inheritdoc />
        public async Task InvokeAsync(CreateUserContext context, 
            PipelineDelegate<CreateUserContext> next, 
            CancellationToken cancellationToken)
        {
            var id = await repository.SaveUserAsync(context.Model, cancellationToken);

            context.EntityId = id;

            await next.InvokeAsync(context, cancellationToken);
        }
    }
}