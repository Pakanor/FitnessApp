using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Interfaces
{
    public interface IProductsCatalogeService
    {
        Task<List<ProductLogEntry>> GetRecentLogsAsync();
        Task<bool> HasAnyLogsAsync();
    }
}
