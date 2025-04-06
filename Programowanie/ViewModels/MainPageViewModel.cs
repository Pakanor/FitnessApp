using FitnessApp.Models;
using FitnessApp.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FitnessApp.ViewModels
{
    public class ProductsCatalogeViewModel : BaseViewModel
    {
        private readonly IProductsCatalogeService _catalogeService;

        public ObservableCollection<ProductLogEntry> ProductLogs { get; set; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _emptyMessage;
        public string EmptyMessage
        {
            get => _emptyMessage;
            set => SetProperty(ref _emptyMessage, value);
        }

        public ProductsCatalogeViewModel(IProductsCatalogeService catalogeService)
        {
            _catalogeService = catalogeService;
        }

        public async Task LoadLogsAsync()
        {
            IsLoading = true;
            EmptyMessage = string.Empty;

            var logs = await _catalogeService.GetRecentLogsAsync();

            ProductLogs.Clear();

            if (logs.Any())
            {
                foreach (var log in logs)
                    ProductLogs.Add(log);
            }
            else
            {
                EmptyMessage = "Brak zapisanych produktów.";
            }

            IsLoading = false;
        }
    }
}
