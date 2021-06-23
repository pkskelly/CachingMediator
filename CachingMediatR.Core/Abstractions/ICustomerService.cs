using System.Collections.Generic;
using System.Threading.Tasks;
using CachingMediatR.Core.Entities;

namespace CachingMediatR.Core.Abstractions
{
    public interface ICustomerService
    {
         Task<List<Customer>> GetCustomerList();
         Task<Customer> GetCustomer(int id);
    }
}