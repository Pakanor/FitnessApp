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

namespace Programowanie
{

    







public partial class MainWindow : Window
    {
        private FilterInfoCollection videoDevices; // Lista kamer
        private VideoCaptureDevice videoSource;   // Wybrana kamera
        private BarcodeReader<Bitmap> barcodeReader;      // Czytnik kodów kreskowych
        private bool isBarcodeScanned = false;
        public MainWindow()
        {
            InitializeComponent();
           
        }


        public async Task<dynamic> GetProductFromApi(string barcode)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";

                    var response = await client.GetStringAsync(url);

                    // Zdeserializowanie odpowiedzi JSON do obiektu dynamicznego
                    dynamic apiResponse = JsonConvert.DeserializeObject(response);

                    // Sprawdzamy, czy odpowiedź zawiera produkty
                    if (apiResponse != null && apiResponse.product != null)
                    {
                        return apiResponse.product; // Zwracamy produkt
                    }
                    else
                    {
                        Console.WriteLine("Brak danych produktu.");
                        return null; // Jeśli nie znaleziono produktu
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd: {ex.Message}");
                    return null; // W przypadku błędu
                }
            }
        }





        private void InitializeBarcodeReader()
        {
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.EAN_13,
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.UPC_A
            }
                }
            };
            Console.WriteLine("BarcodeReader został zainicjalizowany.5");
        }

        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            InitializeCamera();
            ResetUI("scan");
            isBarcodeScanned = false;
        }



        private void ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResultTextBlock.Visibility = Visibility.Visible;
            ResultTextBlock.Text = "Wpisujesz: " + ProductName.Text;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ResetUI("add_product");

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
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();  // Zatrzymanie kamery
                    videoSource = null;          // Zwalniamy zasoby
                }


            }
            else if(Int == "start")
            {

                startPanel.Visibility = Visibility.Visible;
                InputPanel.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Collapsed;
                ProductName.Text = "";
                ResultTextBlock.Visibility = Visibility.Collapsed;
                BarcodeResult.Text = "";


            }
            else if (Int == "scan")
            {
                CameraPreview.Visibility = Visibility.Visible;
                BarcodeResult.Text = "";
                BrandResult.Text = "";

            }


        }



        private void InitializeCamera()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Brak dostępnych kamer.");
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += Video_NewFrame;
                videoSource.Start();
                InitializeBarcodeReader();
                isBarcodeScanned = false; 


                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    Bitmap grayBitmap = ConvertToGrayscale(bitmap);

                    Dispatcher.Invoke(() =>
                    {
                        CameraPreview.Source = ConvertBitmapToBitmapImage(grayBitmap);
                        DecodeBarcode(bitmap);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd w przetwarzaniu klatki: " + ex.Message);
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

                        // Otrzymanie produktu za pomocą API
                        var product = await GetProductFromApi(result.Text);

                        // Sprawdzenie, czy produkt istnieje
                        if (product != null)
                        {
                            // Wyświetlanie danych produktu
                            isBarcodeScanned = true;

                           
                            BarcodeResult.Text = "\nNazwa: " + product.product_name;
                            BrandResult.Text = "\nNazwa: " + product.brands;
                        }
                        else
                        {
                            isBarcodeScanned = true;

                            BarcodeResult.Text = "Błąd";
                            BrandResult.Text = "Zeskanuj ponownie";

                        }


                        // Wyłączenie kamery po zeskanowaniu kodu

                        if (videoSource != null && videoSource.IsRunning)
                        {
                            videoSource.SignalToStop();  // Zatrzymanie kamery
                            videoSource = null;
                        }
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
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }


        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            // Utwórz nowy bitmap obiekt w odcieniach szarości
            Bitmap grayscaleBitmap = new Bitmap(original.Width, original.Height);
            using (Graphics g = Graphics.FromImage(grayscaleBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
            new float[] {0.3f, 0.3f, 0.3f, 0, 0},
            new float[] {0.59f, 0.59f, 0.59f, 0, 0},
            new float[] {0.11f, 0.11f, 0.11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
                });

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return grayscaleBitmap;
        }



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }



        }
    }
}