using BackendLogicApi.Models;
using System;

namespace BackendLogicApi.Interfaces
{
    public interface IProductsCatalogeService
    {
        Task<List<ProductLogEntry>> GetRecentLogsAsync();
        Task<bool> HasAnyLogsAsync();
    }
}
