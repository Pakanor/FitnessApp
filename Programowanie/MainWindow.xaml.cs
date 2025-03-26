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
using System.Windows.Input;

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

        public void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _controller.ProductList_SelectionChanged(sender, e);
        }

        public void ProductList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _controller.ProductList_PreviewMouseLeftButtonDown(sender, e);
        }
        // Buttons to start scanning
        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartScanning();  
        }

        // Text changes 
        private async void ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _controller.OnProductNameChanged();  
        }

        //Turns on the UI for adding product
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            _controller.AddProduct();  
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.BackToStart();  
        }

        
    }


}