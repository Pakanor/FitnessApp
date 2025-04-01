using System.Windows.Input;
using Programowanie.Interfaces;
using Programowanie.Models;
using Programowanie.Services;

namespace Programowanie.ViewModels
{
    public class ProductDetailsViewModel : BaseViewModel
    {
        private readonly ICalorieCalculatorService _calorieService;
        private Product _product;
        private Nutriments _calculatedNutriments;
        private double _userWeight;

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

        public ProductDetailsViewModel(ICalorieCalculatorService calorieService, Product selectedProduct)
        {
            _calorieService = calorieService;
            Product = selectedProduct ?? new Product { Nutriments = new Nutriments() };
            _userWeight = 100; // Domyślna wartość dla przeliczeń
            CalculateNutrition();
        }

        private void CalculateNutrition()
        {
            if (Product != null)
            {
                CalculatedNutriments = _calorieService.CalculateForWeight(Product, UserWeight);
            }
        }
    }
}
