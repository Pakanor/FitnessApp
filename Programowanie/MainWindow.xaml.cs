using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using System.Windows.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Windows.Threading;
using Programowanie.Services;
using Programowanie.Interfaces;
using Programowanie.ViewModels;
using Programowanie.Controllers;
using Programowanie.Helpers;

namespace Programowanie
{

    public partial class MainWindow : Window
    {
        private ICameraService _cameraService; // Zmienna przechowująca instancję serwisu kamery
        private MainViewModel _viewModel;
        private BarcodeReaderService _barcodeReaderService;
        private readonly MainWindowController _controller;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _cameraService = new CameraService();
            _cameraService.FrameReceived += CameraService_FrameReceived;
            _barcodeReaderService = new BarcodeReaderService();
            _barcodeReaderService.BarcodeDetected += OnBarcodeDetected;
            _controller = new MainWindowController(this, _viewModel);
        }

        private void CameraService_FrameReceived(object sender, System.Drawing.Bitmap e)
        {
            // Kiedy otrzymujemy nową klatkę, aktualizujemy interfejs użytkownika
            Dispatcher.Invoke(() =>
            {
                CameraPreview.Source = _barcodeReaderService.ConvertBitmapToBitmapImage(e); // Aktualizacja obrazu
                _barcodeReaderService.DecodeBarcode(e); // Wywołanie skanowania
            });
        }

        private async void OnBarcodeDetected(object sender, string barcode)
        {
            // Wywołanie po wykryciu kodu kreskowego
            await Dispatcher.Invoke(async () =>
            {
                BarcodeResult.Text = "Kod: " + barcode;
                await _viewModel.LoadProductByBarcode(barcode);
                _cameraService.StopCamera();  // Zatrzymanie kamery
            });
        }

        // Przycisk uruchamiający skanowanie
        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartScanning();  // Wywołanie logiki skanowania w kontrolerze
        }

        // Obsługuje zmiany w polu tekstowym
        private async void ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _controller.OnProductNameChanged();  // Zlecenie obsługi w kontrolerze
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            _controller.AddProduct();  // Dodanie produktu
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.BackToStart();  // Powrót do ekranu startowego
        }

        // Zatrzymanie kamery przy zamknięciu aplikacji
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (_cameraService != null)
            {
                _cameraService.StopCamera();  // Zatrzymaj kamerę, jeśli jest aktywna
            }
        }
    }


}