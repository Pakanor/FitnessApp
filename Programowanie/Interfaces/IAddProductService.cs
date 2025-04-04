using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Interfaces
{
    public interface IAddProductService
    {
        Task AddUserLogAsync(Product product, double grams, Nutriments calculated);
    }

}
