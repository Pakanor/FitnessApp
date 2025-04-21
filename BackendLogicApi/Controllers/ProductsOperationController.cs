using BackendLogicApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BackendLogicApi.Models;

namespace BackendLogicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsOperationController : ControllerBase
    {
        private readonly IProductOperationsService _productsOperationService;
        private readonly ICalorieCalculatorService _caloreieCalculatorService;


        public ProductsOperationController(IProductOperationsService productsOperationService, ICalorieCalculatorService calorieCalculatorService )
        {
            _productsOperationService = productsOperationService;
            _caloreieCalculatorService = calorieCalculatorService;

        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentProducts()
        {
            var products = await _productsOperationService.GetRecentLogsAsync();

            if (products == null || !products.Any())
                return NotFound("No products found.");

            return Ok(products);
        }

        
        [HttpPost("add")]
        public async Task<IActionResult> AddUserLog([FromBody] AddLogRequest request)
        {
                var calculated = _caloreieCalculatorService.CalculateForWeight(request.Product, request.Grams);

                if (calculated == null)
                {
                    
                return BadRequest(new { message = "Brak wartości odżywczych błąd." });
            }

             await _productsOperationService.AddUserLogAsync(request.Product, request.Grams, calculated);


            return Ok("Produkt dodany");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserLog(int id)
        {
            await _productsOperationService.DeleteUserLogAsync(id);
            return Ok("Produkt usunięty");
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserLog([FromBody] ProductLogEntry updatedEntry)
        {
            try
            {
                await _productsOperationService.UpdateUserLogAsync(updatedEntry);
                return Ok("Produkt zedytowany");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


    }
}







