using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Enums;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.ServicesInterfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, Guid? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, ProductStatus? status = null);
        Task<Product> GetProductByIdAsync(Guid productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid productId);
        Task UpdateProductStatusAsync(Guid productId, ProductStatus newStatus);
        Task UpdateProductCategoryAsync(Guid productId, Guid newCategoryId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
        Task BatchDeleteAsync(IEnumerable<Guid> productIds); // New batch delete method
        

        }
}
