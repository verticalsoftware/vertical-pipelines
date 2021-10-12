using System;
using System.Threading.Tasks;

namespace Vertical.Examples.Shared
{
    public interface IRepository
    {
        Task<Guid> SaveCustomerAsync(Customer customer);
    }
}