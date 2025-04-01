using Programowanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programowanie.Interfaces
{
    public interface ICalorieCalculatorService
    {
        Nutriments CalculateForWeight(Product product, double grams);

    }
}
