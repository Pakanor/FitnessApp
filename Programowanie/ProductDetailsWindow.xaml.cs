using System.Windows;
using Programowanie.Models; // Upewnij się, że masz dostęp do klasy Product
using Programowanie.ViewModels;
using Programowanie.Services;

namespace Programowanie
{
    public partial class ProductDetailsWindow : Window
    {
        public Product SelectedProduct { get; private set; }
        public int Grams { get; private set; } // Wartość wpisana przez użytkownika

        public ProductDetailsWindow(Product product)
        {
            InitializeComponent();
            SelectedProduct = product;
            DataContext = new ProductDetailsViewModel(new CalorieCalculatorService(), product);
            MessageBox.Show(product.ProductName);
        }

        
    }
}
