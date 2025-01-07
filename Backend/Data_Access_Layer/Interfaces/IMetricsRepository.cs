using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Enums;
using Data_Access_Layer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IMetricsRepository
    {
        Task<int> GetTotalProductsAsync();  // Total count of products
        Task<int> GetTotalCategoriesAsync();  // Total count of categories
        Task<Dictionary<string, int>> GetProductsPerCategoryAsync();  // Products per category
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);  // Low stock alerts
        Task<IEnumerable<CategoryStockChartData>> GetCategoryStockChartDataAsync();  // Chart data for categories and stock
        Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int pageSize = 5);  // Recent activity log
    }
}
