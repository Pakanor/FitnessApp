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
        private MainWindow mainWindow;


        public ProductDetailsWindow(Product product, double grams = 100)
        {
            InitializeComponent();
            var calorieService = new CalorieCalculatorService();
            var productOperationsService = new ProductOperationsService();
            mainWindow = new MainWindow();
            if (product == null)
            {
                IsEditMode = false;
                SelectedProduct = new Product();
            }
            else
            {
                IsEditMode = true;
                SelectedProduct = product; 
            }

            DataContext = new ProductDetailsViewModel(calorieService, SelectedProduct, productOperationsService, IsEditMode, grams);


            MessageBox.Show(product?.ProductName ?? "Nowy produkt");
        }
    }
}