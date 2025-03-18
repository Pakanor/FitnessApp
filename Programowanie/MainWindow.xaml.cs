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

namespace Programowanie
{

    public partial class MainWindow : Window
    {

        private ICameraService _cameraService; // Zmienna przechowująca instancję serwisu kamery

        private BarcodeReader<Bitmap> barcodeReader;      // Czytnik kodów kreskowych
        private ProductServiceAPI _productService;
        private bool isBarcodeScanned = false;
        private MainViewModel _viewModel;
        private BarcodeReaderService _barcodeReaderService;
        

        public MainWindow()
        {
            InitializeComponent();
            _productService = new ProductServiceAPI();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _cameraService = new CameraService();
            _cameraService.FrameReceived += CameraService_FrameReceived;
            _barcodeReaderService = new BarcodeReaderService();
            _barcodeReaderService.BarcodeDetected += OnBarcodeDetected;
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
            _ = Dispatcher.Invoke(async () =>
            {
                BarcodeResult.Text = "Kod: " + barcode;
                await _viewModel.LoadProductByBarcode(barcode);
                _cameraService.StopCamera();  
            });
        }

        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            _cameraService.StartCamera();
            ResetUI("scan");
            isBarcodeScanned = false;

        }
        public class Debouncer
        {
            private DispatcherTimer _timer;
            private Action _action;

            public Debouncer(int milliseconds)
            {
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(milliseconds)
                };
                _timer.Tick += (s, e) =>
                {
                    _timer.Stop();
                    _action?.Invoke();
                };
            }

            public void Debounce(Action action)
            {
                _action = action;
                _timer.Stop();
                _timer.Start();
            }
        }



        //wzorce projektowe-----------------------------------------------------------------



        private readonly Debouncer _debouncer = new Debouncer(500); // Opóźnienie 500ms

        private async void ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProductNameChange.Visibility = Visibility.Visible;
            ProductName.Text = "Wpisujesz: " + ProductNameChange.Text;
            _debouncer.Debounce(async () =>
            {
                string productName = ProductNameChange.Text.Trim();
                if (productName.Length > 3)
                {
                    _ = _viewModel.LoadProductByName(productName);

                }
            });



        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ResetUI("add_product");
            _cameraService.StopCamera();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI("start");
        }


        private void ResetUI(string Int)
        {
            if (Int == "add_product")
            {
                startPanel.Visibility = Visibility.Collapsed;
                InputPanel.Visibility = Visibility.Visible;
                BackButton.Visibility = Visibility.Visible;
                BarcodeResult.Text = "";
                CameraPreview.Visibility = Visibility.Collapsed;



            }
            else if (Int == "start")
            {

                startPanel.Visibility = Visibility.Visible;
                InputPanel.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Collapsed;

                BarcodeResult.Text = "";


            }
            else if (Int == "scan")
            {
                CameraPreview.Visibility = Visibility.Visible;
                BarcodeResult.Text = "";

            }


        }



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Zatrzymaj kamerę, jeśli jest aktywna
            if (_cameraService != null)
            {
                _cameraService.StopCamera();
            }
        }
    }
}