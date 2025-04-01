using System.Windows;
using System.Windows.Input;
using FitnessApp.Interfaces;
using FitnessApp.Services;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using FitnessApp.Services;

namespace FitnessApp.ViewModels
{
    public class ProductDetailsViewModel : BaseViewModel
    {
        private readonly ICalorieCalculatorService _calorieService;
        private Product _product;
        private Nutriments _calculatedNutriments;
        private double _userWeight;
        private readonly IAddProductService _addProductService;

        public Product NewProduct { get; set; } = new Product(); // Tworzymy nowy produkt do edycji

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
                    CalculateNutrition(); // Automatycznie obliczaj po zmianie wartości!
                }
            }
        }

        public ProductDetailsViewModel(ICalorieCalculatorService calorieService, Product selectedProduct, IAddProductService addProductService)
        {
            _calorieService = calorieService;
            Product = selectedProduct ?? new Product { Nutriments = new Nutriments() };
            _userWeight = 100; // Domyślna wartość dla przeliczeń
            _addProductService = addProductService;
            AddProductCommand = new RelayCommand(AddProduct);
            CalculateNutrition();
        }

        private void CalculateNutrition()
        {
            if (Product != null)
            {
                CalculatedNutriments = _calorieService.CalculateForWeight(Product, UserWeight);
            }
        }
        private void AddProduct()
        {
            _addProductService.AddProduct(NewProduct);
            NewProduct = new Product(); // Czyścimy produkt po dodaniu
            OnPropertyChanged(nameof(NewProduct));
            MessageBox.Show("dodano");
        }
    }
}
