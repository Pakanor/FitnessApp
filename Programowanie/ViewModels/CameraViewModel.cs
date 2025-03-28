using Programowanie.Interfaces;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Programowanie.ViewModels
{
    public class CameraViewModel : BaseViewModel
    {
        private readonly ICameraService _cameraService;
        private readonly BarcodeReaderService _barcodeReaderService;
        private string _barcodeResult;
        private BitmapImage _cameraPreviewImage;
        private Dispatcher _dispatcher; // Nowa zmienna na Dispatcher


        public string BarcodeResult
        {
            get => _barcodeResult;
            set => SetProperty(ref _barcodeResult, value);
        }

        public BitmapImage CameraPreviewImage
        {
            get => _cameraPreviewImage;
            set => SetProperty(ref _cameraPreviewImage, value);
        }
        public void SetDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public CameraViewModel(ICameraService cameraService, BarcodeReaderService barcodeReaderService)
        {
            _cameraService = cameraService;
            _barcodeReaderService = barcodeReaderService;

            // Subskrypcja zdarzeń
            _cameraService.FrameReceived += CameraService_FrameReceived;
            _barcodeReaderService.BarcodeDetected += OnBarcodeDetected;
        }

        private void CameraService_FrameReceived(object sender, Bitmap e)
        {
            // Wywołanie z Dispatcher.Invoke na głównym wątku, aby zaktualizować UI
            _dispatcher?.Invoke(() =>
            {
                CameraPreviewImage = _barcodeReaderService.ConvertBitmapToBitmapImage(e);
                _barcodeReaderService.DecodeBarcode(e);
            });
        }

        private void OnBarcodeDetected(object sender, string barcode)
        {
            BarcodeResult = "Kod: " + barcode;

            // Tutaj przekazujesz informację do MainViewModel, zamiast do ProductViewModel
            BarcodeDetected?.Invoke(this, barcode); // Wydarzenie informujące o znalezionym kodzie
        }

        // Komunikat o wykryciu kodu
        public event EventHandler<string> BarcodeDetected;

        public void StartCamera()
        {
            _cameraService.StartCamera();
        }

        public void StopCamera()
        {
            _cameraService.StopCamera();
        }
    }
}