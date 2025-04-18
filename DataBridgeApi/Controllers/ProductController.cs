using Microsoft.AspNetCore.Mvc;
using FitnessApp.Interfaces;
using FitnessApp.Models;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ICalorieCalculatorService _calorieService;

        public ProductController(ICalorieCalculatorService calorieService)
        {
            _calorieService = calorieService;
        }

        [HttpPost("calculate")]
        public IActionResult CalculateCalories([FromBody] ProductCalculationRequest request)
        {
            if (request.Product == null || request.Weight <= 0)
                return BadRequest("Invalid data.");

            var result = _calorieService.CalculateForWeight(request.Product, request.Weight);
            return Ok(result);
        }
    }

    public class ProductCalculationRequest
    {
        public Product Product { get; set; }
        public double Weight { get; set; }
    }
}
