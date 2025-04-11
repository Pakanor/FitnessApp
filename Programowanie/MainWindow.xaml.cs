using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FitnessApp.DataAccess;
using FitnessApp.Helpers;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using FitnessApp.Services;
using FitnessApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private bool _userClicked = false;
        private ProductViewModel _Products;
        private readonly Debouncer _debouncer = new Debouncer(500);
        private readonly IProductsCatalogeService _catalogeService;





        public MainWindow()
        {
            InitializeComponent();

            _catalogeService = new ProductsCatalogeService(new ProductLogRepository(new AppDbContext()));

            _viewModel = new MainViewModel(_catalogeService);
            this.DataContext = _viewModel;
            _viewModel.CameraViewModel.SetDispatcher(this.Dispatcher);
            _Products = _viewModel.ProductViewModel;
            LoadProducts();



        }

        public void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_userClicked || e.AddedItems.Count == 0) return; 

            if (ProductList.SelectedItem is Product selectedProduct)
            {
                var detailsWindow = new ProductDetailsWindow(selectedProduct);

                if (detailsWindow.ShowDialog() == true)
                {
                    MessageBox.Show($"Wybrano: {selectedProduct.ProductName}, Ilość: {detailsWindow.Grams}g");
                }

                ProductList.SelectedItem = null; 
            }

            _userClicked = false;
        }

        private async void LoadProducts()
        {
            await _viewModel.LoadProductsAsync();
        }
       public void ProductList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
{
    var listBox = sender as ListBox;

    var clickedItem = GetListBoxItemUnderMouse(listBox, e);
    if (clickedItem != null)
    {
        var clickedProduct = clickedItem.DataContext as Product;

        if (clickedProduct != null && clickedProduct == listBox.SelectedItem)
        {
            var detailsWindow = new ProductDetailsWindow(clickedProduct);
            if (detailsWindow.ShowDialog() == true)
            {
                MessageBox.Show($"Wybrano: {clickedProduct.ProductName}, Ilość: {detailsWindow.Grams}g");
            }

            listBox.SelectedItem = null;
            e.Handled = true;
        }
        else
        {
            _userClicked = true; 
        }
    }
}

private ListBoxItem GetListBoxItemUnderMouse(ListBox listBox, MouseButtonEventArgs e)
{
    var point = e.GetPosition(listBox);
    var element = listBox.InputHitTest(point) as DependencyObject;

    while (element != null && !(element is ListBoxItem))
    {
        element = VisualTreeHelper.GetParent(element);
    }

    return element as ListBoxItem;
}

        private void StartScanning_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StartScanning();
        }

        

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddProduct(); 
            LoadProducts();

        }
        private async void ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _debouncer.Debounce(() =>
            {
                Dispatcher.Invoke(async () =>
                {

                    string productName = ProductNameChange.Text.Trim();
                    if (productName.Length > 3)
                    {
                        _ = _Products.LoadProductByName(productName);

                    }
                });
            });
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.BackToStart(); 
            LoadProducts();

        }
    }
}