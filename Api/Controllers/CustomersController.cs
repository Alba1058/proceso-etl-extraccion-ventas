using Domain.Entities.Api;
using Api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/source/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerSourceRepository _customerSourceRepository;

        public CustomersController(ICustomerSourceRepository customerSourceRepository)
        {
            _customerSourceRepository = customerSourceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerApiDto>>> GetCustomers(CancellationToken cancellationToken)
        {
            var customers = await _customerSourceRepository.GetAllCustomersAsync(cancellationToken);
            return Ok(customers);
        }
    }
}
