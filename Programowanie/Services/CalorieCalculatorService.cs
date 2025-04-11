using FitnessApp.Interfaces;
using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Services
{

    //calculator for calories
    public class CalorieCalculatorService : ICalorieCalculatorService
    {
        public Nutriments CalculateForWeight(Product product, double grams)
        {
            if (product?.Nutriments == null)
                return null;

            double factor = grams / 100.0;
            return new Nutriments
            {
                Energy = product.Nutriments.Energy * factor,
                Fat = product.Nutriments.Fat * factor,
                Carbs = product.Nutriments.Carbs * factor,
                Proteins = product.Nutriments.Proteins * factor,
                Salt = product.Nutriments.Salt * factor,
                EnergyUnit = product.Nutriments.EnergyUnit
            };
        }
    }
}
