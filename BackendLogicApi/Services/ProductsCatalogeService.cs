using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLogicApi.Services
{
    public class ProductsCatalogeService : IProductsCatalogeService // public zamiast internal
    {
        // Wyświetlanie wszystkich produktów
        private readonly ProductLogRepository _logRepository;

        public ProductsCatalogeService(ProductLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<List<ProductLogEntry>> GetRecentLogsAsync()
        {
            return await _logRepository.GetAllAsync();
        }

        public async Task<bool> HasAnyLogsAsync()
        {
            var logs = await _logRepository.GetAllAsync();
            return logs.Any();
        }
    }
}
