using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipelinesExample.Models;
using PipelinesExample.Pipeline;
using Vertical.Pipelines;

namespace PipelinesExample.Services
{
    public class UserRepository
    {
        private readonly ILogger<UserRepository> logger;
        private readonly ConcurrentDictionary<Guid, UserModel> data = new();

        public UserRepository(ILogger<UserRepository> logger)
        {
            this.logger = logger;
        }

        public Task<Guid> SaveUserAsync(UserModel model, 
            CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            
            // "Save" user record
            data[id] = model;
            logger.LogInformation("Saved user record, generated id = {UserId}", id);

            return Task.FromResult(id);
        }
    }
}