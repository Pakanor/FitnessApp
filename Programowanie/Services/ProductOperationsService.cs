using FitnessApp.DataAccess;
using FitnessApp.Models;
using FitnessApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FitnessApp.Services
{
    public class ProductOperationsService
    {
        private readonly List<Product> _products;
        private readonly ProductLogRepository _repository;
        private readonly Window _window;

        public Product NewProduct { get; set; } = new Product();

        public ProductOperationsService()
        {

            var context = new AppDbContext();
            _repository = new ProductLogRepository(context);
            _products = new List<Product>();
        }

        public async Task AddUserLogAsync(Product product, double grams, Nutriments calculated)
        {
            if (product == null || calculated == null)
                throw new ArgumentNullException();

            var entry = new ProductLogEntry
            {
                ProductName = product.ProductName,
                Brands = product.Brands,
                Grams = grams,
                Energy = calculated.Energy,
                Fat = calculated.Fat,
                Sugars = calculated.Carbs,
                Proteins = calculated.Proteins,
                Salt = calculated.Salt,
                EnergyUnit = "Gr",
                LoggedAt = DateTime.UtcNow
            };

            await _repository.AddLogEntryAsync(entry);
        }

        public async Task DeleteUserLogAsync(ProductLogEntry log) {

            if (log == null)
                throw new ArgumentNullException(nameof(log));

            // Ewentualnie dodatkowa logika, np. sprawdzenie uprawnień

            await _repository.DeleteAsync(log);
            MessageBox.Show("dzialaaa");


        }


    }
}
