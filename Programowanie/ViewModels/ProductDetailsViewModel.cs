﻿using System.Windows;
using System.Windows.Input;
using FitnessApp.Interfaces;
using FitnessApp.Services;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using FitnessApp.Services;
using FitnessApp.DataAccess;

namespace FitnessApp.ViewModels
{
    public class ProductDetailsViewModel : BaseViewModel
    {
        private Product _product;
        private Nutriments _calculatedNutriments;
        private double _userWeight;
        private readonly ProductOperationsService _productOperationsService;
        private readonly ProductLogRepository _repository;
        private MainViewModel _viewModel;
        public event Action ProductSaved;
        private MainWindow window;

        private readonly ApiClient _apiClient = new ApiClient("http://localhost:5142");



        public bool IsEditMode { get; set; }


        public ICommand SaveCommand { get; }

        public ICommand AddProductCommand { get; }
        public Product Product
        {
            get => _product;
            set { _product = value; OnPropertyChanged(nameof(Product)); }
        }

        public Nutriments CalculatedNutriments
        {
            get => _calculatedNutriments;
            private set { _calculatedNutriments = value; OnPropertyChanged(nameof(CalculatedNutriments)); }
        }

        public double UserWeight
        {
            get => _userWeight;
            set
            {
                if (_userWeight != value)
                {
                    _userWeight = value;
                    OnPropertyChanged(nameof(UserWeight));
                    CalculateNutrition(); 
                }
            }
        }

        public ProductDetailsViewModel( Product selectedProduct, ProductOperationsService productOperationsService, bool isEditMode, double grams)
        {
            Product = selectedProduct ?? new Product { Nutriments = new Nutriments() };
            _userWeight = 100; 
            _productOperationsService = productOperationsService;

            CalculateNutrition();
            _repository = new ProductLogRepository(new AppDbContext()); 
                    IsEditMode = isEditMode;
            UserWeight = grams > 0 ? grams : 100;
            SaveCommand = new RelayCommand(SaveProduct);
                       

            _viewModel = new MainViewModel();



        }

        private async void CalculateNutrition()
        {
            if (Product != null)
            {
                var request = new CalculationRequest
                {
                    Product = Product,
                    Grams = UserWeight
                };

                try
                {
                    CalculatedNutriments = await _apiClient.PostAsync<CalculationRequest, Nutriments>("api/caloriecalculator/calculate", request);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas obliczania kalorii: {ex.Message}");
                }
            }
        }


        private async void SaveProduct()
        {
            if (Product == null || CalculatedNutriments == null)
            {
                MessageBox.Show("Brakuje danych.");
                return;
            }

            var entry = new ProductLogEntry
            {
                Id = Product.Id,
                ProductName = Product.ProductName,
                Brands = Product.Brands,
                Grams = UserWeight,
                Energy = CalculatedNutriments.Energy,
                Fat = CalculatedNutriments.Fat,
                Sugars = CalculatedNutriments.Carbs,
                Proteins = CalculatedNutriments.Proteins,
                Salt = CalculatedNutriments.Salt,
                EnergyUnit = CalculatedNutriments.EnergyUnit
            };

            if (IsEditMode && entry.Id > 0)
            {
                await _repository.UpdateAsync(entry);
                MessageBox.Show("edycja");
                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();


            }
            else
            {
                await _productOperationsService.AddUserLogAsync(Product, UserWeight, CalculatedNutriments);
                MessageBox.Show("dodanie");
               
                _viewModel.LoadProductsAsync();
                
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                var productDetailsWindow = Application.Current.Windows
   .OfType<ProductDetailsWindow>()
   .FirstOrDefault();
               
                productDetailsWindow?.Close();

               
            });

        }

    }
}
