﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Models;
using FitnessApp.Services;
using System.Windows;
using Newtonsoft.Json;

namespace FitnessApp.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly ProductServiceAPI _productService;
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private string _errorMessage;
        private string _productName;
       

        public string ProductName
        {
            get => _productName;
            set
            {
                if (_productName != value)
                {
                    _productName = value;
                    OnPropertyChanged(nameof(ProductName)); // Powiadomienie o zmianie
                }
            }
        }

        // Metoda wyświetlająca MessageBox z tekstem


        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products)); // Powiadomienie o zmianie listy
            }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        
        public ProductViewModel()
        {
            _productService = new ProductServiceAPI();
            Products = new ObservableCollection<Product> { };
        }

        // Ładowanie produktu po kodzie kreskowym
        public async Task LoadProductByBarcode(string barcode)
        {
            try
            {
                ErrorMessage = "";
                var product = await _productService.GetProductFromApiBarcode(barcode);
                if (product != null)
                {
                    SelectedProduct = product;
                }
                else
                {
                    ErrorMessage = "Product not found!";
                    MessageBox.Show("Product not found!");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error scanning barcode: " + ex.Message;
                MessageBox.Show($"Error scanning barcode: {ex.Message}");
            }
        }

        // Ładowanie produktów po nazwie
        public async Task LoadProductByName(string name)
        {
            try
            {
                ErrorMessage = "";

                var productList = await _productService.GetProductFromApiName(name);

                if (productList != null)
                {
                    // Pokazujemy komunikat, ile produktów zwróciło API
                    MessageBox.Show($"API zwróciło {productList.Count} produktów.", "Informacja");

                    // Użyj Dispatcher, aby zaktualizować kolekcję w głównym wątku UI
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Czyścimy dotychczasowe produkty
                        Products.Clear();

                        // Dodajemy wszystkie nowe produkty
                        foreach (var product in productList)
                        {
                            Products.Add(product);
                            // MessageBox.Show(product.ProductName);

                        }
                        MessageBox.Show($"Liczba produktów w kolekcji: {Products.Count}");


                        // Jeśli produkty zostały dodane, ustawiamy pierwszy jako wybrany
                        SelectedProduct =Products.FirstOrDefault();
                    });
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

    }
}