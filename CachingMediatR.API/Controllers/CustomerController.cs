using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachingMediatR.Core.Features.Customers.GetCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CachingMediatR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomers(int id){
            var customer = await _mediator.Send(new GetCustomerQuery {Id = id, BypassCache = false });
            return Ok(customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers(){
            var customers = await _mediator.Send(new GetCustomerListQuery {BypassCache = false });
            return Ok(customers);
        }

    }
}
