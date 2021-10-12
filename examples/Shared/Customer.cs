using System;

namespace Vertical.Examples.Shared
{
    public class Customer
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTimeOffset DateModified { get; set; } = DateTimeOffset.UtcNow;
    }
}