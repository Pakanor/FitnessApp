using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using System.Threading.Tasks;
using FitnessApp.Interfaces;
using FitnessApp.ViewModels;
using FitnessApp.Services;
namespace FitnessApp.Services
{
    public class BarcodeReaderService : IBarcodeService
    {
        private BarcodeReader barcodeReader;
        private bool isBarcodeScanned = false;

        public event EventHandler<string> BarcodeDetected; // Event, który powiadomi MainWindow

        public BarcodeReaderService()
        {
            InitializeBarcodeReader();
        }

        public void InitializeBarcodeReader()
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
        }

        public void DecodeBarcode(Bitmap bitmap)
        {
            if (isBarcodeScanned)
                return;

            if (barcodeReader == null)
            {
                InitializeBarcodeReader();
            }

            try
            {
                var result = barcodeReader.Decode(bitmap);

                if (result != null)
                {
                    isBarcodeScanned = true;
                    System.Windows.MessageBox.Show($"Zeskanowano kod: {result.Text}");

                    BarcodeDetected?.Invoke(this, result.Text); // Powiadomienie UI o zeskanowanym kodzie
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd dekodowania kodu: " + ex.Message);
            }
        }

        public BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}