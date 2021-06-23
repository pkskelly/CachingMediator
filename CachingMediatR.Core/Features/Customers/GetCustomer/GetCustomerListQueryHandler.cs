using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CachingMediatR.Core.Abstractions;
using CachingMediatR.Core.Entities;
using MediatR;

namespace CachingMediatR.Core.Features.Customers.GetCustomer
{
    public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, List<Customer>>
    {
        private readonly ICustomerService _customerService;
        public GetCustomerListQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<List<Customer>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerService.GetCustomerList();
            return customers;
        }
    }
}