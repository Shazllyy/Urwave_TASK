using Business_Logic_Layer.Enums;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
        Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status);
        Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize,
           Guid? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null,
           ProductStatus? status = null);
        Task<IEnumerable<Product>> GetSortedProductsAsync(string sortBy, bool isDescending = false);
        Task BatchDeleteAsync(IEnumerable<Guid> productIds);  // Added batch delete
        Task UpdateProductStatusAsync(Guid productId, ProductStatus newStatus); // Added status management
        Task UpdateProductCategoryAsync(Guid productId, Guid newCategoryId);  // Added category assignment


    }
}
