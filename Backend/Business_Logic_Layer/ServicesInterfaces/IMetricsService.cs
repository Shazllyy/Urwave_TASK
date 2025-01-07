using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.ServicesInterfaces
{
    public interface IMetricsService
    {
        Task<int> GetTotalProductsAsync();  // Get total product count
        Task<int> GetTotalCategoriesAsync();  // Get total category count
        Task<Dictionary<string, int>> GetProductsPerCategoryAsync();  // Get products per category
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);  // Get low stock products
        Task<IEnumerable<CategoryStockChartData>> GetCategoryStockChartDataAsync();  // Get chart data for category stock
        Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int pageSize = 5);  // Get recent activity log
    }
}
