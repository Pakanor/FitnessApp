using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading.Tasks;
using Programowanie.Services;
using System.Windows;
using Newtonsoft.Json;
namespace Programowanie.ViewModels

{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ProductServiceAPI _productService;
        private string _productName;
        private string _productBrand;
        private string _errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
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
            try
            {
                ErrorMessage = ""; // Resetowanie błędu
                var product = await _productService.GetProductFromApiBarcode(barcode);
                if (product != null)
                {
                    ProductBrand = product.brands;
                    ProductName = product.product_name;
                }
                else
                {
                    ErrorMessage = "Product not found!";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error scanning barcode: " + ex.Message;
            }
        }

        public async Task LoadProductByName(string name)
        {
            try
            {
                ErrorMessage = ""; // Resetowanie błędu
                var product = await _productService.GetProductFromApiName(name);

                if (product != null)
                {

                    ProductBrand = product.brands;
                    ProductName = product.product_name;

                }
                else
                {
                    MessageBox.Show("Produkt nie znaleziony!", "Błąd");
                    ErrorMessage = "Product not found!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd w LoadProductByName:\n{ex.Message}", "Błąd");
                ErrorMessage = "Error searching product: " + ex.Message;
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
