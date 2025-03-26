using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Programowanie.Models;


namespace Programowanie.ViewModels
{
    internal class ProductViewModel:BaseViewModel
    {
        private Product _product;

        public ProductViewModel(Product product)
        {
            _product = product;

        }
        public string ProductName => _product.ProductName;
        public string Brands => _product.Brands;
        public double Energy => _product.Nutriments?.Energy ?? 0;
        public string EnergyUnit => _product.Nutriments?.EnergyUnit ?? "kcal";
    }
}
