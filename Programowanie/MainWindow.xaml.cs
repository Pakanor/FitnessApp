using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FitnessApp.DataAccess;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using FitnessApp.Services;
using FitnessApp.ViewModels;

namespace FitnessApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private readonly IProductsCatalogeService _catalogeService;

        public MainWindow()
        {
            InitializeComponent();

            _catalogeService = new ProductsCatalogeService(new ProductLogRepository(new AppDbContext()));

            _viewModel = new MainViewModel(_catalogeService);
            this.DataContext = _viewModel;
            _viewModel.CameraViewModel.SetDispatcher(this.Dispatcher);
            _viewModel.LoadProductsAsync();



        }

        private void ProductLogItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item && item.DataContext is Product product)
            {
                if (DataContext is MainViewModel vm)
                {
                    _viewModel.ProductClickedCommand.Execute(product);
                    e.Handled = true;
                }
            }
        }


        private void ProductListItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item && item.DataContext is Product product)
            {
                if (DataContext is MainViewModel vm)
                {
                    _viewModel.ProductClickedCommand.Execute(product);
                    e.Handled = true;
                }
            }
        }







    }
}