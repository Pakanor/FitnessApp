using BackendLogicApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BackendLogicApi.Models;
using BackendLogicApi.DataAccess;

namespace BackendLogicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsOperationController : ControllerBase
    {
        private readonly IProductOperationsService _productsOperationService;
        private readonly ICalorieCalculatorService _caloreieCalculatorService;
        private readonly ILogger<ProductsOperationController> _logger; 


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

            return Ok(products); // Zwraca dane w formacie JSON
        }

        
        [HttpPost("add")]
        public async Task<IActionResult> AddUserLog([FromBody] AddLogRequest request)
        {
                var calculated = _caloreieCalculatorService.CalculateForWeight(request.Product, request.Grams);

                if (calculated == null)
                {
                    _logger.LogError("Calculated nutrients are missing.");
                return BadRequest(new { message = "Calculated nutrients are missing." });
            }

             await _productsOperationService.AddUserLogAsync(request.Product, request.Grams, calculated);


            return Ok("Product added to log.");
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserLog(int id)
        {
            await _productsOperationService.DeleteUserLogAsync(id);
            return Ok("Product log deleted.");
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserLog([FromBody] ProductLogEntry updatedEntry)
        {
            try
            {
                await _productsOperationService.UpdateUserLogAsync(updatedEntry);
                return Ok("Product log updated.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


    }
}







