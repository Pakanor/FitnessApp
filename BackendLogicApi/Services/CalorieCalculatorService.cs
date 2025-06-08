using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendLogicApi.Services
{

    //calculator for calories
   
    public class CalorieCalculatorService : ICalorieCalculatorService
    {
        private readonly ICalorieCalculatorService _calculator;

        public Nutriments CalculateForWeight(Product product, double grams)
        {
            if (product?.Nutriments == null)
                return null;

            double factor = grams / 100.0;

            return new Nutriments
            {
                Energy = Math.Round(product.Nutriments.Energy * factor, 2),
                Fat = Math.Round(product.Nutriments.Fat * factor, 2),
                Carbs = Math.Round(product.Nutriments.Carbs * factor, 2),
                Proteins = Math.Round(product.Nutriments.Proteins * factor, 2),
                Salt = Math.Round(product.Nutriments.Salt * factor, 2),
                EnergyUnit = product.Nutriments.EnergyUnit
            };
        }

    }
}
