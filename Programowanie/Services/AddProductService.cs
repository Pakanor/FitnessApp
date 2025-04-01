using FitnessApp.Interfaces;
using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FitnessApp.Services
{
    public class AddProductService : IAddProductService
    {
        private readonly List<Product> _products;

        public AddProductService()
        {
            _products = new List<Product>();
        }

        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _products.Add(product); //logika do bazy
        }
    }
}
