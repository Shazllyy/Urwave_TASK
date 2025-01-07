using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Enums;
using Business_Logic_Layer.ServicesInterfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Uow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;

        private const string ProductCacheKey = "Products";

        public ProductService(IUnitOfWork unitOfWork, IMemoryCache cache, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        // Helper method to invalidate paginated caches
        private void InvalidatePaginatedCache()
        {
            int totalPages = 10; // Adjust this based on expected total pages
            int pageSize = 100; // Default or configured page size

            for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
            {
                var cacheKey = $"{ProductCacheKey}_Page_{pageNumber}_Size_{pageSize}";
                _cache.Remove(cacheKey);
            }
        }

        // Get products with pagination and caching
        public async Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, Guid? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, ProductStatus? status = null)
        {
            var cacheKey = $"{ProductCacheKey}_Page_{pageNumber}_Size_{pageSize}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Product> products))
            {
                products = await _unitOfWork.Product.GetProductsAsync(pageNumber, pageSize, categoryId, minPrice, maxPrice, status);
                _cache.Set(cacheKey, products, TimeSpan.FromMinutes(10));
                _logger.LogInformation("Cached product list for key: {CacheKey}", cacheKey);
            }
            return products;
        }

        // Get a product by ID with individual caching
        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            var productCacheKey = $"Product_{productId}";

            if (!_cache.TryGetValue(productCacheKey, out Product product))
            {
                product = await _unitOfWork.Product.GetByIdAsync(productId, p => p.Category)
                         ?? throw new Exception("Product not found.");
                _cache.Set(productCacheKey, product, TimeSpan.FromMinutes(10));
            }

            return product;
        }

        // Add a product to the database and invalidate the cache
        public async Task AddProductAsync(Product product)
        {
            try
            {
                await _unitOfWork.Product.AddAsync(product);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Product added: {ProductName}", product.Name);

                _cache.Remove(ProductCacheKey);
                InvalidatePaginatedCache();

                var productCacheKey = $"Product_{product.Id}";
                _cache.Remove(productCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the product: {ProductName}", product.Name);
                throw new ApplicationException("An error occurred while adding the product.", ex);
            }
        }

        // Update product in the database and invalidate the cache
        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Product.Update(product);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Product updated: {ProductId}", product.Id);

            _cache.Remove(ProductCacheKey);
            InvalidatePaginatedCache();

            var productCacheKey = $"Product_{product.Id}";
            _cache.Remove(productCacheKey);
        }

        // Batch delete products and invalidate cache
        public async Task BatchDeleteAsync(IEnumerable<Guid> productIds)
        {
            if (productIds == null || !productIds.Any())
                throw new ArgumentException("Product IDs cannot be null or empty.");

            await _unitOfWork.Product.BatchDeleteAsync(productIds);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Batch delete completed for product IDs: {ProductIds}", productIds);

            _cache.Remove(ProductCacheKey);
            InvalidatePaginatedCache();
        }

        // Delete a product and invalidate the cache
        public async Task DeleteProductAsync(Guid productId)
        {
            var product = await _unitOfWork.Product.GetByIdAsync(productId);
            if (product == null) throw new Exception("Product not found.");

            _unitOfWork.Product.Remove(product);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Product deleted: {ProductId}", productId);

            _cache.Remove(ProductCacheKey);
            InvalidatePaginatedCache();

            var productCacheKey = $"Product_{productId}";
            _cache.Remove(productCacheKey);
        }

        // Update the product status and invalidate cache
        public async Task UpdateProductStatusAsync(Guid productId, ProductStatus newStatus)
        {
            await _unitOfWork.Product.UpdateProductStatusAsync(productId, newStatus);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Product status updated: {ProductId}, New Status: {Status}", productId, newStatus);

            _cache.Remove(ProductCacheKey);
            InvalidatePaginatedCache();

            var productCacheKey = $"Product_{productId}";
            _cache.Remove(productCacheKey);
        }

        // Update the product category and invalidate cache
        public async Task UpdateProductCategoryAsync(Guid productId, Guid newCategoryId)
        {
            await _unitOfWork.Product.UpdateProductCategoryAsync(productId, newCategoryId);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Product category updated: {ProductId}, New Category: {CategoryId}", productId, newCategoryId);

            _cache.Remove(ProductCacheKey);
            InvalidatePaginatedCache();

            var productCacheKey = $"Product_{productId}";
            _cache.Remove(productCacheKey);
        }

        // Get products that are low in stock
        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            return await _unitOfWork.Product.GetLowStockProductsAsync(threshold);
        }
    }
}
