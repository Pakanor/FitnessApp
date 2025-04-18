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

        public async Task AddLogEntryAsync(ProductLogEntry entry)
        {
            _context.ProductLogEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductLogEntry>> GetAllAsync()
        {
            return await _context.ProductLogEntries.OrderByDescending(e => e.LoggedAt).ToListAsync();
        }
        public async Task DeleteAsync(ProductLogEntry entry)
        {
            _context.ProductLogEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductLogEntry updatedEntry)
        {
            var existingEntry = await _context.ProductLogEntries
                .FirstOrDefaultAsync(e => e.Id == updatedEntry.Id); 

            if (existingEntry != null)
            {
                existingEntry.ProductName = updatedEntry.ProductName;
                existingEntry.Brands = updatedEntry.Brands;
                existingEntry.Grams = updatedEntry.Grams;
                existingEntry.Energy = updatedEntry.Energy;
                existingEntry.Fat = updatedEntry.Fat;
                existingEntry.Sugars = updatedEntry.Sugars;
                existingEntry.Proteins = updatedEntry.Proteins;
                existingEntry.Salt = updatedEntry.Salt;
                existingEntry.EnergyUnit = updatedEntry.EnergyUnit;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Nie znaleziono produktu do edycji.", nameof(updatedEntry));
            }
        }
    }
}
