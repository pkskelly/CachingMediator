using System.Threading;
using System.Threading.Tasks;
using CachingMediatR.Core.Abstractions;
using CachingMediatR.Core.Entities;
using MediatR;

namespace CachingMediatR.Core.Features.Customers.GetCustomer
{
    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Customer>
    {
        private readonly ICustomerService _customerService;
        public GetCustomerQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerService.GetCustomer(request.Id);
            return customer;
        }
    }
}