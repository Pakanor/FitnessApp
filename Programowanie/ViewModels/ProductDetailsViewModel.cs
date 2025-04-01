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
            set { _userWeight = value; OnPropertyChanged(nameof(UserWeight)); }
        }

        public ICommand CalculateCommand { get; }

        public ProductDetailsViewModel(ICalorieCalculatorService calorieService, Product selectedProduct)
        {
            _calorieService = calorieService;
            Product = selectedProduct;
            CalculateCommand = new RelayCommand(CalculateNutrition);
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
