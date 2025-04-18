using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;


namespace BackendLogicApi.Services
{
    public class ProductOperationsService: IProductOperationsService
    {
        private readonly List<Product> _products;
        private readonly ProductLogRepository _repository;
        private readonly AppDbContext _context;

        public Product NewProduct { get; set; } = new Product();

        public ProductOperationsService(AppDbContext context)
        {

            _context = context;
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


            await _repository.DeleteAsync(log);


        }
        

    }
}
