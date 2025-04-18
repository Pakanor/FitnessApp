using FitnessApp.DataAccess;
using FitnessApp.Interfaces;
using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Services
{
    internal class ProductsCatalogeService : IProductsCatalogeService
    {
        //dipslaying all Products
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
