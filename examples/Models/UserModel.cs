using System;

namespace PipelinesExample.Models
{
    public record UserModel(
        string FirstName,
        string LastName,
        string EmailAddress);
}