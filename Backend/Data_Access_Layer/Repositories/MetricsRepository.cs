using Data_Access_Layer.ApplicationDbContext;
using Data_Access_Layer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class MetricsRepository : IMetricsRepository
    {
        private readonly ApplicationDbcontext _context;

        public MetricsRepository(ApplicationDbcontext context)
        {
            _context = context;
        }

        // Get total count of products
        public async Task<int> GetTotalProductsAsync()
        {
            return await _context.Products.CountAsync();
        }

        // Get total count of categories
        public async Task<int> GetTotalCategoriesAsync()
        {
            return await _context.Category.CountAsync();
        }

        // Get products per category
        public async Task<Dictionary<string, int>> GetProductsPerCategoryAsync()
        {
            var productsPerCategory = await _context.Products
                .GroupBy(p => p.Category.Name)  // Grouping by category name
                .Select(g => new { CategoryName = g.Key, ProductCount = g.Count() })
                .ToListAsync();

            // Convert to Dictionary
            return productsPerCategory.ToDictionary(p => p.CategoryName, p => p.ProductCount);
        }

        // Get products with low stock (threshold can be defined)
        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            return await _context.Products
                .Where(p => p.StockQuantity <= threshold)
                .ToListAsync();
        }

        // Get data for the category stock chart (example: quantity of products per category)
        public async Task<IEnumerable<CategoryStockChartData>> GetCategoryStockChartDataAsync()
        {
            var data = await _context.Category
                .Select(c => new CategoryStockChartData
                {
                    CategoryName = c.Name,
                    StockQuantity = _context.Products.Where(p => p.CategoryId == c.Id).Sum(p => p.StockQuantity)
                })
                .ToListAsync();

            return data;
        }

        // Get recent activity log (e.g., recent changes, updates, or actions in the system)
        public async Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int pageSize = 5)
        {
            return await _context.ActivityLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}