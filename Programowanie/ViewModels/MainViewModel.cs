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
using System.Collections.ObjectModel;
using Programowanie.Models;
namespace Programowanie.ViewModels

{
    public class MainViewModel : BaseViewModel
    {

        private readonly ProductServiceAPI _productService;
        private string _productName;
        private string _productBrand;

        private string _errorMessage;
        private Product _selectedProduct;

        //For binding
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }
        //For binding


        public string ProductBrand
        {
            get => _productBrand;
            set => SetProperty(ref _productBrand, value);
        }


        //Using for List in UI
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products)); // Powiadomienie o zmianie listy
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }


        public MainViewModel()
        {
            _productService = new ProductServiceAPI();

        }

        public async Task LoadProductByBarcode(string barcode)
        {

            try
            {
                ErrorMessage = ""; 
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
                ErrorMessage = ""; 
                var productList = await _productService.GetProductFromApiName(name);

                Products.Clear();
                MessageBox.Show(JsonConvert.SerializeObject(productList, Formatting.Indented));

                foreach (var product in productList)
                {
                    Products.Add(product);

                }

                if (Products.Any())
                {

                    SelectedProduct = Products.First(); 
                }
                else
                {
                    ErrorMessage = "Product not found!";
                    MessageBox.Show("Produkt nie znaleziony!", "Błąd");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error searching product: " + ex.Message;
                MessageBox.Show($"Błąd w LoadProductByName:\n{ex.Message}", "Błąd");
            }
        }






        // Setting the 
       

    }
}
