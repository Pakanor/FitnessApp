using FitnessApp.Helpers;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FitnessApp.Services;

namespace FitnessApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly CameraViewModel _cameraViewModel;

        private readonly ProductViewModel _productViewModel;
        private readonly UIStateManager _uiStateManager;




        public CameraViewModel CameraViewModel => _cameraViewModel;
        public ProductViewModel ProductViewModel => _productViewModel;

        public UIStateManager UIStateManager => _uiStateManager;

        public ICommand StartScanningCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand BackToStartCommand { get; }
        public MainViewModel()
        {
            ICameraService cameraService = new CameraService();
            BarcodeReaderService barcodeReaderService = new BarcodeReaderService();

            _cameraViewModel = new CameraViewModel(cameraService, barcodeReaderService);
            _productViewModel = new ProductViewModel();
            _uiStateManager = new UIStateManager();


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


    }

}