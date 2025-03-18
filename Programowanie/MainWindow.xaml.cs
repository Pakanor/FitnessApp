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

        public MainWindow()
        {
            InitializeComponent();
            _productService = new ProductServiceAPI();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _cameraService = new CameraService();
            _cameraService.FrameReceived += CameraService_FrameReceived;
        }
        private void CameraService_FrameReceived(object sender, System.Drawing.Bitmap e)
        {
            // Kiedy otrzymujemy nową klatkę, aktualizujemy interfejs użytkownika
            Dispatcher.Invoke(() =>
            {
                CameraPreview.Source = ConvertBitmapToBitmapImage(e); // Aktualizacja obrazu na UI
                DecodeBarcode(e); // Aktualizacja obrazu na UI
            });
        }





        private void InitializeBarcodeReader()
        {
            if (barcodeReader == null) // Zapobiega wielokrotnej inicjalizacji
            {
                var options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.EAN_13,
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.UPC_A
            }
                };

                barcodeReader = new BarcodeReader
                {
                    AutoRotate = true,
                    Options = options
                };

                Console.WriteLine("BarcodeReader został zainicjalizowany.");
            }
            else
            {
                Console.WriteLine("BarcodeReader był już zainicjalizowany.");
            }
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
                    //var product = await _productService.GetProductFromApiName(productName);
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
            else if(Int == "start")
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




        private void DecodeBarcode(Bitmap bitmap)
        {
            try
            {
                if (isBarcodeScanned)
                    return;
                if (barcodeReader == null)
                {
                    InitializeBarcodeReader();
                }

                var result = barcodeReader.Decode(bitmap);

                if (result != null)
                {
                    Dispatcher.Invoke(async () =>
                    {
                        BarcodeResult.Text = "Kod: " + result.Text;

                        await _viewModel.LoadProductByBarcode(result.Text);
                        isBarcodeScanned = true;

                        // Zatrzymujemy kamerę, jeśli kod został zeskanowany
                        _cameraService.StopCamera();  // Używamy serwisu do zatrzymania kamery
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd dekodowania kodu: " + ex.Message);
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            // Tworzymy strumień pamięci, w którym zapiszemy dane obrazu.
            using (MemoryStream memory = new MemoryStream())
            {
                // Zapisujemy obiekt Bitmap do strumienia pamięci w formacie BMP.
                bitmap.Save(memory, ImageFormat.Bmp);

                // Ustawiamy pozycję strumienia na początek, aby móc go później odczytać.
                memory.Position = 0;

                // Tworzymy nowy obiekt BitmapImage, który będzie zawierał obraz.
                BitmapImage bitmapImage = new BitmapImage();

                // Inicjujemy obraz w asynchroniczny sposób.
                bitmapImage.BeginInit();

                // Przypisujemy strumień pamięci jako źródło danych obrazu.
                bitmapImage.StreamSource = memory;

                // Wybieramy opcję przechowywania obrazu w pamięci, co umożliwia jego natychmiastowe załadowanie.
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                // Kończymy inicjalizację obrazu.
                bitmapImage.EndInit();

                // Zwracamy przygotowany obiekt BitmapImage.
                return bitmapImage;
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