using Business_Logic_Layer.Enums;
using Data_Access_Layer.ApplicationDbContext;
using Data_Access_Layer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbcontext _context;
        private readonly DbSet<Product> _dbSet;

        public ProductRepository(ApplicationDbcontext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Product>();
        }

        // Get products by CategoryId
        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        // Get low stock products
        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            return await _dbSet.Where(p => p.StockQuantity < threshold).ToListAsync();
        }

        // Get products by status
        public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status)
        {
            return await _dbSet.Where(p => p.Status == status).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize,
          Guid? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null,
          ProductStatus? status = null)
        {
            
            var query = _dbSet.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            query = query.Include(p => p.Category) // Assuming a navigation property to Category
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetSortedProductsAsync(string sortBy, bool isDescending = false)
        {
            var query = _dbSet.AsQueryable();

            // Custom sorting logic
            switch (sortBy)
            {
                case "Name":
                    query = isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                    break;
                case "Price":
                    query = isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                    break;
                default:
                    break;
            }

            return await query.ToListAsync();
        }
        // Batch delete products
        public async Task BatchDeleteAsync(IEnumerable<Guid> productIds)
        {
            var productsToDelete = await _dbSet.Where(p => productIds.Contains(p.Id)).ToListAsync();
            _dbSet.RemoveRange(productsToDelete);
            await Task.CompletedTask;
        }

        // Update product status
        public async Task UpdateProductStatusAsync(Guid productId, ProductStatus newStatus)
        {
            var product = await _dbSet.FindAsync(productId);
            if (product != null)
            {
                product.Status = newStatus;
                _dbSet.Update(product);
                await Task.CompletedTask;
            }
            else
            {
                throw new Exception("Product not found.");
            }
        }

        public async Task UpdateProductCategoryAsync(Guid productId, Guid newCategoryId)
        {
            var product = await _dbSet.FindAsync(productId);
            if (product != null)
            {
                product.CategoryId = newCategoryId;
                _dbSet.Update(product);
                await Task.CompletedTask;
            }
            else
            {
                throw new Exception("Product not found.");
            }
        }
    }
}
