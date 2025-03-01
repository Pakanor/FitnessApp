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
            Console.WriteLine("BarcodeReader został zainicjalizowany.");
        }

        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            InitializeCamera();
        }



        private void ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Przypisujemy tekst z TextBox do TextBlock, aby wyświetlić na bieżąco to, co użytkownik wpisuje
            ResultTextBlock.Visibility = Visibility.Visible;
            ResultTextBlock.Text = "Wpisujesz: " + ProductName.Text;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Przechodzimy na stronę formularza z "InputPanel"
            startPanel.Visibility = Visibility.Collapsed;  
            InputPanel.Visibility = Visibility.Visible;   
            BackButton.Visibility = Visibility.Visible;     
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Cofamy do strony głównej
            startPanel.Visibility = Visibility.Visible;    
            InputPanel.Visibility = Visibility.Collapsed;  
            BackButton.Visibility = Visibility.Collapsed;  
            ProductName.Text = "";
            ResultTextBlock.Visibility = Visibility.Collapsed;
            


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

                if (barcodeReader == null)
                {
                    InitializeBarcodeReader();
                }
                if (isBarcodeScanned)
                    return;
                var result = barcodeReader.Decode(bitmap);
               
                if (result != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        BarcodeResult.Text = "Kod: " + result.Text;
                        Console.WriteLine("Zeskanowany kod: " + result.Text);
                        isBarcodeScanned = true;


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