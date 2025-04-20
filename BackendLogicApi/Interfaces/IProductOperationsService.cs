using BackendLogicApi.Models;

namespace BackendLogicApi.Interfaces
{
    public interface IProductOperationsService
    {
        Task AddUserLogAsync(Product product, double grams, Nutriments calculated);

        Task DeleteUserLogAsync(ProductLogEntry log);
        Task<List<ProductLogEntry>> GetRecentLogsAsync();
        Task<bool> HasAnyLogsAsync();

    }
}