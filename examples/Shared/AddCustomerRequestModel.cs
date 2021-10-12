using System;

namespace Vertical.Examples.Shared
{
    public class AddCustomerRequestModel
    {
        public Customer Record { get; set; }
        public Guid NewId { get; set; }
    }
}