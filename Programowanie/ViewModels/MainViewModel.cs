using FitnessApp.Helpers;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FitnessApp.Services;
using System.Windows.Controls;

namespace FitnessApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly CameraViewModel _cameraViewModel;
        private bool _userClicked = false;

        private readonly ProductViewModel _productViewModel;
        private readonly UIStateManager _uiStateManager;
        private readonly IProductsCatalogeService _catalogeService;
        public string EmptyMessage { get; set; }
        public CameraViewModel CameraViewModel => _cameraViewModel;
        public ProductViewModel ProductViewModel => _productViewModel;
        public UIStateManager UIStateManager => _uiStateManager;
        public ObservableCollection<ProductLogEntry> ProductLogs { get; set; } = new();



        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }


        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    OnPropertyChanged(nameof(SelectedProduct));

                    if (_userClicked && _selectedProduct != null)
                    {
                        ShowDetails(_selectedProduct);
                    }
                }
            }
        }

        public ICommand StartScanningCommand { get; }

        public ICommand AddProductCommand { get; }

        public ICommand BackToStartCommand { get; }
        public ICommand DeleteProductLogCommand { get; }
        public ICommand EditProductLogCommand { get; }




        public MainViewModel(IProductsCatalogeService catalogeService)
        {
            ICameraService cameraService = new CameraService();
            BarcodeReaderService barcodeReaderService = new BarcodeReaderService();

            _cameraViewModel = new CameraViewModel(cameraService, barcodeReaderService);
            _productViewModel = new ProductViewModel();
            _uiStateManager = new UIStateManager();
            _catalogeService = catalogeService;
            DeleteProductLogCommand = new RelayCommand<ProductLogEntry>(DeleteProductLog);
            EditProductLogCommand = new RelayCommand<ProductLogEntry>(EditProductLog);



            // Subskrypcja na zdarzenie BarcodeDetected z CameraViewModel
            _cameraViewModel.BarcodeDetected += OnBarcodeDetected;

            // Komendy
            StartScanningCommand = new RelayCommand(StartScanning);
            AddProductCommand = new RelayCommand(AddProduct);
            BackToStartCommand = new RelayCommand(BackToStart);
        }

        // Obsługuje zdarzenie, gdy kod kreskowy zostanie wykryty
        private void OnBarcodeDetected(object sender, string barcode)
        {
            // Załadowanie produktu po zeskanowanym kodzie
            _productViewModel.LoadProductByBarcode(barcode);
        }

        private void DeleteProductLog(ProductLogEntry log)
        {
            if (log != null)
            {
                MessageBox.Show($"Log: {log.ProductName}");
            }
            else
            {
                MessageBox.Show("blad");

            }
        }
        private void EditProductLog(ProductLogEntry log)
        {
            if (log != null)
            {
                MessageBox.Show($"Log: {log.ProductName}");
            }
            else
            {
                MessageBox.Show("blad");

            }
        }

        private void ShowDetails(Product product)
        {
            var detailsWindow = new ProductDetailsWindow(product);

            if (detailsWindow.ShowDialog() == true)
            {
                MessageBox.Show($"Wybrano: {product.ProductName}, Ilość: {detailsWindow.Grams}g");
            }

            SelectedProduct = null;
            _userClicked = false;
        }



        // Rozpoczęcie skanowania
        public void StartScanning()
        {
            CameraViewModel.StartCamera();
            UIStateManager.SetScanMode(); // Przełączamy na tryb skanowania

        }

        // Zatrzymanie skanowania
        public void StopScanning()
        {
            CameraViewModel.StopCamera();
        }
        public void AddProduct()
        {
            CameraViewModel.StopCamera();
            UIStateManager.SetInputMode(); // Przełączamy na tryb wpisywania produktu

        }
        public void BackToStart()
        {
            CameraViewModel.StopCamera();
            UIStateManager.SetStartMode(); // Przełączamy na tryb startowy

        }

        public async Task LoadProductsAsync()
        {
            IsLoading = true;
            EmptyMessage = string.Empty;

            var logs = await _catalogeService.GetRecentLogsAsync();

            ProductLogs.Clear();

            if (logs.Any())
            {
                foreach (var log in logs)
                    ProductLogs.Add(log);
            }
            else
            {
                EmptyMessage = "Brak zapisanych produktów.";
            }

            IsLoading = false;
        }



    }

}