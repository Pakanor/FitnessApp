using System.Windows;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using FitnessApp.Services;
using FitnessApp.Services;

namespace FitnessApp
{
    public partial class ProductDetailsWindow : Window
    {
        public Product SelectedProduct { get; private set; }
        public int Grams { get; private set; } // Wartość wpisana przez użytkownika

        public ProductDetailsWindow(Product product)
        {
            InitializeComponent();
            var calorieService = new CalorieCalculatorService();
            var addProductService = new AddProductService(this);
            SelectedProduct = product;
            DataContext = new ProductDetailsViewModel(calorieService, product, addProductService);
            MessageBox.Show(product.ProductName);
        }

        
    }
}
