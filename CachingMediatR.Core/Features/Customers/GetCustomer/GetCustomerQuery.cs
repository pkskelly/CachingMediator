using System;
using CachingMediatR.Core.Abstractions;
using CachingMediatR.Core.Entities;
using MediatR;

namespace CachingMediatR.Core.Features.Customers.GetCustomer
{
    public class GetCustomerQuery : IRequest<Customer>, ICacheableMediatrQuery
    {
        public int Id { get; set; }
        public bool BypassCache { get; set; }
        public string CacheKey => $"Customer - {Id}";
        public TimeSpan? SlidingExpiration { get; set; }
    }
}