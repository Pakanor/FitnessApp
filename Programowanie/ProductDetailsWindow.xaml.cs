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
        public bool IsEditMode { get; set; } // Flaga trybu edycji


        public ProductDetailsWindow(Product product, double grams = 100)
        {
            InitializeComponent();
            var calorieService = new CalorieCalculatorService();
            var addProductService = new ProductOperationsService();
            if (product == null)
            {
                IsEditMode = false; // tryb dodawania
                SelectedProduct = new Product(); // Pusty produkt do dodania
            }
            else
            {
                IsEditMode = true; // tryb edycji
                SelectedProduct = product; // Edytujemy przekazany produkt
            }

            DataContext = new ProductDetailsViewModel(calorieService, SelectedProduct, addProductService, IsEditMode, grams);
            MessageBox.Show(product?.ProductName ?? "Nowy produkt");
        }
    }
}