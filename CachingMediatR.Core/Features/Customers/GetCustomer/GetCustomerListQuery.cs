using System;
using System.Collections.Generic;
using CachingMediatR.Core.Abstractions;
using CachingMediatR.Core.Entities;
using MediatR;

namespace CachingMediatR.Core.Features.Customers.GetCustomer
{
    public class GetCustomerListQuery : IRequest<List<Customer>>, ICacheableMediatrQuery
    {
        public int Id { get; set; }
        public bool BypassCache { get; set; }
        public string CacheKey => $"CustomerList";
        public TimeSpan? SlidingExpiration { get; set; }
    }
}