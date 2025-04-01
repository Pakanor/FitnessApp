using Programowanie.Interfaces;
using Programowanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programowanie.Services
{
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
                Sugars = product.Nutriments.Sugars * factor,
                Proteins = product.Nutriments.Proteins * factor,
                Salt = product.Nutriments.Salt * factor,
                EnergyUnit = product.Nutriments.EnergyUnit
            };
        }
    }
}
