using FitnessApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.DataAccess
{
    public class ProductLogRepository
    {
        private readonly AppDbContext _context;

        public ProductLogRepository(AppDbContext context)
        {
            _context = context;
        }

        //adding product to db
        public async Task AddLogEntryAsync(ProductLogEntry entry)
        {
            _context.ProductLogEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        //selecting product from db
        public async Task<List<ProductLogEntry>> GetAllAsync()
        {
            return await _context.ProductLogEntries.OrderByDescending(e => e.LoggedAt).ToListAsync();
        }
    }
}
