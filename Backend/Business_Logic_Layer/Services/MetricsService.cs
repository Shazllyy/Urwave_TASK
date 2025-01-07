using Business_Logic_Layer.ServicesInterfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Uow;
using DataAccessLayer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class MetricsService : IMetricsService
    {
        private readonly IUnitOfWork _unitOfWork;  // Use UnitOfWork
        private readonly ILogger<MetricsService> _logger;

        public MetricsService(IUnitOfWork unitOfWork, ILogger<MetricsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Get total number of products
        public async Task<int> GetTotalProductsAsync()
        {
            try
            {
                return await _unitOfWork.MetricsRepository.GetTotalProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting total products.");
                throw;
            }
        }

        // Get total number of categories
        public async Task<int> GetTotalCategoriesAsync()
        {
            try
            {
                return await _unitOfWork.MetricsRepository.GetTotalCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting total categories.");
                throw;
            }
        }

        // Get products per category (returns a dictionary of category name and product count)
        public async Task<Dictionary<string, int>> GetProductsPerCategoryAsync()
        {
            try
            {
                return await _unitOfWork.MetricsRepository.GetProductsPerCategoryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting products per category.");
                throw;
            }
        }

        // Get low stock products based on threshold
        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            try
            {
                return await _unitOfWork.MetricsRepository.GetLowStockProductsAsync(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting low stock products.");
                throw;
            }
        }

        // Get category stock chart data for analytics (example: chart showing categories and their stock levels)
        public async Task<IEnumerable<CategoryStockChartData>> GetCategoryStockChartDataAsync()
        {
            try
            {
                return await _unitOfWork.MetricsRepository.GetCategoryStockChartDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting category stock chart data.");
                throw;
            }
        }

        // Get recent activity logs
        public async Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int pageSize = 5)
        {
            try
            {
                return await _unitOfWork.MetricsRepository.GetRecentActivitiesAsync(pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting recent activities.");
                throw;
            }
        }
    }
}
