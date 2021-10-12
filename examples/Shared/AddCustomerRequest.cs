using System;

namespace Vertical.Examples.Shared
{
    public class AddCustomerRequest
    {
        public Customer Record { get; set; }
        public Guid NewId { get; set; }
    }
}