using System.Windows;
using Programowanie.Models; // Upewnij się, że masz dostęp do klasy Product

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
            DataContext = product; // Powiązanie danych z UI
            MessageBox.Show(product.ProductName);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(GramsTextBox.Text, out int grams))
            {
                Grams = grams;
                DialogResult = true;

                Close();
            }
            else
            {
                MessageBox.Show("Podaj poprawną liczbę gramów.");
            }
        }
    }
}
