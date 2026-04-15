using Domain.Entities.Api;
using Api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/source/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductSourceRepository _productSourceRepository;

        public ProductsController(IProductSourceRepository productSourceRepository)
        {
            _productSourceRepository = productSourceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductApiDto>>> GetProducts(CancellationToken cancellationToken)
        {
            var products = await _productSourceRepository.GetAllProductsAsync(cancellationToken);
            return Ok(products);
        }
    }
}
