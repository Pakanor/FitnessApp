using System.Windows;
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
        private readonly ICalorieCalculatorService _calorieService;
        private Product _product;
        private Nutriments _calculatedNutriments;
        private double _userWeight;
        private readonly ProductOperationsService _productOperationsService;
        private readonly ProductLogRepository _repository;
        private MainViewModel _viewModel;
        private readonly IProductsCatalogeService _catalogeService;


        public Product NewProduct { get; set; } = new Product(); // Tworzymy nowy produkt do edycji
        public bool IsEditMode { get; set; }

        public event Action ProductSaved;

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
                    CalculateNutrition(); // Automatycznie obliczaj po zmianie wartości!
                }
            }
        }

        public ProductDetailsViewModel(ICalorieCalculatorService calorieService, Product selectedProduct, ProductOperationsService productOperationsService, bool isEditMode, double grams)
        {
            _calorieService = calorieService;
            Product = selectedProduct ?? new Product { Nutriments = new Nutriments() };
            _userWeight = 100; // Domyślna wartość dla przeliczeń
            _productOperationsService = productOperationsService;

            CalculateNutrition();
            _repository = new ProductLogRepository(new AppDbContext()); 
                    IsEditMode = isEditMode;
            UserWeight = grams > 0 ? grams : 100;
            SaveCommand = new RelayCommand(SaveProduct);
                        _catalogeService = new ProductsCatalogeService(new ProductLogRepository(new AppDbContext()));

            _viewModel = new MainViewModel(_catalogeService);



        }

        private void CalculateNutrition()
        {
            if (Product != null)
            {
                CalculatedNutriments = _calorieService.CalculateForWeight(Product, UserWeight);
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

            }
            else
            {
                await _productOperationsService.AddUserLogAsync(Product, UserWeight, CalculatedNutriments);
                MessageBox.Show("dodanie");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var productDetailsWindow = Application.Current.Windows
       .OfType<ProductDetailsWindow>()
       .FirstOrDefault();
                    productDetailsWindow?.Close();

                    Application.Current.MainWindow.Show();
                });
            }

        }

    }
}
