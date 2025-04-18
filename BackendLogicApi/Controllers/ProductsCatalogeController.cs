using BackendLogicApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendLogicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsCatalogeController : ControllerBase
    {
        private readonly IProductsCatalogeService _productsCatalogeService;

        // Konstruktor, wstrzykiwanie zależności
        public ProductsCatalogeController(IProductsCatalogeService productsCatalogeService)
        {
            _productsCatalogeService = productsCatalogeService;
        }

        // Endpoint do pobierania listy produktów
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentProducts()
        {
            var products = await _productsCatalogeService.GetRecentLogsAsync();

            if (products == null || !products.Any())
                return NotFound("No products found.");

            return Ok(products); // Zwraca dane w formacie JSON
        }
    }
}
