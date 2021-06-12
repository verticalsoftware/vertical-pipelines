using System;
using PipelinesExample.Models;

namespace PipelinesExample.Pipeline
{
    public class CreateUserContext
    {
        public Guid EntityId { get; set; }
        
        public UserModel Model { get; set; }
    }
}