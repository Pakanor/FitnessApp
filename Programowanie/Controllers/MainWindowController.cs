using Programowanie.Interfaces;
using Programowanie.ViewModels;
using System.Drawing;
using System.Windows;
using Programowanie.Services;
using ZXing;
using System.Windows.Threading;
using Programowanie.Helpers;
using System.Windows.Controls;

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


        public MainWindowController(MainWindow view)
        {
            _view = view;
            _viewModel = new MainViewModel();
            _cameraService = new CameraService();
            _barcodeReaderService = new BarcodeReaderService();

            _cameraService.FrameReceived += CameraService_FrameReceived;
            _barcodeReaderService.BarcodeDetected += OnBarcodeDetected;
        }

        private void CameraService_FrameReceived(object sender, Bitmap e)
        {
            _view.Dispatcher.Invoke(() =>
            {
                _view.CameraPreview.Source = _barcodeReaderService.ConvertBitmapToBitmapImage(e);
                _barcodeReaderService.DecodeBarcode(e);
            });
        }

        private async void OnBarcodeDetected(object sender, string barcode)
        {
            await _view.Dispatcher.Invoke(async () =>
            {
                _view.BarcodeResult.Text = "Kod: " + barcode;
                await _viewModel.LoadProductByBarcode(barcode);
                _cameraService.StopCamera();
            });
        }

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
        }

        private void ResetUI(string mode)
        {
            _view.startPanel.Visibility = (mode == "start") ? Visibility.Visible : Visibility.Collapsed;
            _view.InputPanel.Visibility = (mode == "add_product") ? Visibility.Visible : Visibility.Collapsed;
            _view.CameraPreview.Visibility = (mode == "scan") ? Visibility.Visible : Visibility.Collapsed;
            _view.BackButton.Visibility = (mode == "add_product") ? Visibility.Visible : Visibility.Collapsed;
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
