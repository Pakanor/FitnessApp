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
        private MainViewModel _viewModel;
        private readonly MainWindowController _controller;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            _controller = new MainWindowController(this, _viewModel);
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
        
    }


}