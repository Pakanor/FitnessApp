using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading.Tasks;
using Programowanie.Services;
namespace Programowanie.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ProductServiceAPI _productService;
        private string _productName;
        private string _productBrand;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public string ProductBrand
        {
            get => _productBrand;
            set => SetProperty(ref _productBrand, value);
        }


        public MainViewModel()
        {
            _productService = new ProductServiceAPI();

        }

        public async Task LoadProductByBarcode(string barcode)
        {
            var product = await _productService.GetProductFromApiBarcode(barcode);
            if (product != null)
            {
                ProductBrand = product.brands;
                ProductName = product.product_name;
                OnPropertyChanged(null);
            }
        }

        public async Task LoadProductByName(string name)
        {
            var product = await _productService.GetProductFromApiName(name);
            if (product != null)
            {
                ProductBrand = product.brands;
                ProductName = product.product_name;
                OnPropertyChanged(null);
            }
        }






        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
