using Programowanie.Interfaces;
using Programowanie.ViewModels;
using System.Drawing;
using System.Windows;
using Programowanie.Services;
using ZXing;
using System.Windows.Threading;
using Programowanie.Helpers;
using System.Windows.Controls;
using Programowanie.Models;
using System.Windows.Input;
using System.Windows.Media;

namespace Programowanie.Controllers
{
    public class MainWindowController : IDisposable
    {
        private readonly MainWindow _view;
        private readonly ICameraService _cameraService;
        private readonly MainViewModel _viewModel;
        private readonly BarcodeReaderService _barcodeReaderService;
        private bool _disposed = false;
        private readonly Debouncer _debouncer = new Debouncer(500); // Opóźnienie 500ms
        private bool _userClicked = false;

        public MainWindowController(MainWindow view, MainViewModel viewModel)
        {
            _view = view;
            _viewModel = viewModel;
            _cameraService = new CameraService();
            _barcodeReaderService = new BarcodeReaderService();

            _cameraService.FrameReceived += CameraService_FrameReceived;
            _barcodeReaderService.BarcodeDetected += OnBarcodeDetected;
            _view.DataContext = _viewModel;

        }

        public void OnProductNameChanged()
        {
            _view.ProductNameChange.Visibility = Visibility.Visible;

            _debouncer.Debounce(() =>
            {
                _view.Dispatcher.Invoke(async () =>
                {

                    string productName = _view.ProductNameChange.Text.Trim();
                    if (productName.Length > 3)
                    {
                        _ = _viewModel.LoadProductByName(productName);

                    }
                });
            });
        }


        //For working of the camera
        private void CameraService_FrameReceived(object sender, Bitmap e)
        {
            _view.Dispatcher.Invoke(() =>
            {
                _view.CameraPreview.Source = _barcodeReaderService.ConvertBitmapToBitmapImage(e);
                _barcodeReaderService.DecodeBarcode(e);
            });
        }

        //For working of the BarcodeDetection
        private async void OnBarcodeDetected(object sender, string barcode)
        {
            await _view.Dispatcher.Invoke(async () =>
            {
                _view.BarcodeResult.Text = "Kod: " + barcode;
                await _viewModel.LoadProductByBarcode(barcode);
                _cameraService.StopCamera();
            });
        }

        //UI
        public void StartScanning()
        {
            _cameraService.StartCamera();
            ResetUI("scan");
        }

        public void AddProduct()
        {
            ResetUI("add_product");
            _cameraService.StopCamera();
        }

        public void BackToStart()
        {
            ResetUI("start");
            _cameraService.StopCamera();
        }

        public void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_userClicked || e.AddedItems.Count == 0) return; // Ignorujemy zmiany, jeśli nie było kliknięcia

            if (_view.ProductList.SelectedItem is Product selectedProduct)
            {
                var detailsWindow = new ProductDetailsWindow(selectedProduct);

                if (detailsWindow.ShowDialog() == true)
                {
                    MessageBox.Show($"Wybrano: {selectedProduct.ProductName}, Ilość: {detailsWindow.Grams}g");
                }

                _view.ProductList.SelectedItem = null; // Wyczyść zaznaczenie po zamknięciu okna
            }

            _userClicked = false; // Resetujemy flagę po wyborze
        }

        public void ProductList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Sprawdzenie, czy kliknięto na element listy
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is ListBoxItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            if (source != null)
            {
                _userClicked = true; // Ustawiamy flagę tylko jeśli kliknięto w element listy
            }
        }



        private void ResetUI(string mode)
        {
            _view.startPanel.Visibility = (mode == "start") ? Visibility.Visible : Visibility.Collapsed;
            _view.InputPanel.Visibility = (mode == "add_product") ? Visibility.Visible : Visibility.Collapsed;
            _view.CameraPreview.Visibility = (mode == "scan") ? Visibility.Visible : Visibility.Collapsed;
            _view.BackButton.Visibility = (mode == "add_product" || mode == "scan") ? Visibility.Visible : Visibility.Collapsed;
            _view.ProductList.Visibility = (mode == "add_product") ? Visibility.Visible : Visibility.Collapsed;

            _view.BarcodeResult.Text = "";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cameraService.StopCamera();
                _cameraService.FrameReceived -= CameraService_FrameReceived;
                _barcodeReaderService.BarcodeDetected -= OnBarcodeDetected;
                _disposed = true;
            }
        }
    }

}
